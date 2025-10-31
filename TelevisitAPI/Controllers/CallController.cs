using Microsoft.AspNetCore.Mvc;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.TwiML;
using Twilio.TwiML.Voice;
using dotenv.net;
using TelevisitAPI.Data;
using TelevisitAPI.Models;

namespace TelevisitAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CallController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly string _accountSid;
        private readonly string _authToken;
        private readonly string _twilioNumber;

        public CallController(AppDbContext context)
        {
            _context = context;

            // Load environment variables from .env
            DotEnv.Load();

            _accountSid = Environment.GetEnvironmentVariable("TWILIO_ACCOUNT_SID") ?? "";
            _authToken = Environment.GetEnvironmentVariable("TWILIO_AUTH_TOKEN") ?? "";
            _twilioNumber = Environment.GetEnvironmentVariable("TWILIO_PHONE_NUMBER") ?? "";

            if (string.IsNullOrEmpty(_accountSid) || string.IsNullOrEmpty(_authToken) || string.IsNullOrEmpty(_twilioNumber))
            {
                Console.WriteLine("⚠️ Missing Twilio environment variables. Please check your .env file.");
            }
        }

        // ✅ 1️⃣ Endpoint to make a call
        [HttpPost("makecall")]
        public IActionResult MakeCall([FromQuery] string toPhoneNumber)
        {
            try
            {
                TwilioClient.Init(_accountSid, _authToken);

                var ngrokUrl = "https://valentina-unfrightened-hector.ngrok-free.dev"; // 👈 Replace this!

                var call = CallResource.Create(
                    to: new Twilio.Types.PhoneNumber(toPhoneNumber),
                    from: new Twilio.Types.PhoneNumber(_twilioNumber),
                    url: new Uri($"{ngrokUrl}/api/call/voice") // Twilio will hit this route when the call connects
                );

                return Ok(new { message = "Call initiated successfully", sid = call.Sid });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // ✅ 2️⃣ Twilio will call this URL to get call instructions
        [HttpPost("voice")]
        public IActionResult Voice()
        {
            var response = new VoiceResponse();
            var gather = new Gather(numDigits: 1, action: new Uri("/api/call/handle-response", UriKind.Relative), method: "POST");
            gather.Say("You have an appointment tomorrow. Press 1 if you will attend. Press 2 if you will not attend.");
            response.Append(gather);
            response.Say("We did not receive any input. Goodbye.");
            response.Hangup();

            return Content(response.ToString(), "text/xml");
        }

        // ✅ 3️⃣ Handles keypad response (1 or 2)
        [HttpPost("handle-response")]
        public IActionResult HandleResponse([FromForm] string Digits, [FromForm] string To)
        {
            string status = "no_response";

            if (Digits == "1") status = "yes";
            else if (Digits == "2") status = "no";

            // Save to database
            var appointment = _context.Appointments.FirstOrDefault(a => a.PhoneNumber == To);
            if (appointment == null)
            {
                appointment = new Appointment { PhoneNumber = To, Status = status, Timestamp = DateTime.Now };
                _context.Appointments.Add(appointment);
            }
            else
            {
                appointment.Status = status;
                appointment.Timestamp = DateTime.Now;
            }

            _context.SaveChanges();

            // Send TwiML confirmation
            var response = new VoiceResponse();
            if (status == "yes")
                response.Say("Thank you. Your appointment is confirmed. Goodbye.");
            else if (status == "no")
                response.Say("Thank you. We have cancelled your appointment. Goodbye.");
            else
                response.Say("Invalid input. Goodbye.");

            response.Hangup();

            return Content(response.ToString(), "text/xml");
        }
    }
}
