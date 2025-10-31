using Microsoft.AspNetCore.Mvc;
using dotenv.net;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.TwiML;
using Twilio.TwiML.Voice;
using Twilio.Types;
using TelevisitAPI.Data;
using TelevisitAPI.Models;

namespace TelevisitAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TelevisitController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TelevisitController(AppDbContext context)
        {
            _context = context;
            DotEnv.Load();
        }

        // 🟩 Step 1: Initiate the call
        [HttpPost("call")]
        public IActionResult MakeCall([FromBody] string phoneNumber)
        {
            try
            {
                var accountSid = Environment.GetEnvironmentVariable("TWILIO_ACCOUNT_SID");
                var authToken = Environment.GetEnvironmentVariable("TWILIO_AUTH_TOKEN");
                var fromNumber = Environment.GetEnvironmentVariable("TWILIO_PHONE_NUMBER");

                TwilioClient.Init(accountSid, authToken);

                var ngrokUrl = "https://valentina-unfrightened-hector.ngrok-free.dev";

                var call = CallResource.Create(
                    to: new PhoneNumber(phoneNumber),
                    from: new PhoneNumber(fromNumber),
                    url: new Uri($"{ngrokUrl}/api/Televisit/voice")
                );

                var appointment = new Appointment
                {
                    PhoneNumber = phoneNumber,
                    Status = "Call Initiated",
                    Timestamp = DateTime.UtcNow,
                    CallSid = call.Sid
                };

                _context.Appointments.Add(appointment);
                _context.SaveChanges();
                Console.WriteLine($"✅ Saved appointment for {phoneNumber}");


                return Ok(new { Message = "Call initiated successfully!", CallSid = call.Sid });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = "Failed to make the call", Error = ex.Message });
            }
        }

        // 🟩 Step 2: Voice instructions
        [HttpPost("voice")]
        public IActionResult Voice()
        {
            var response = new VoiceResponse();
            var gather = new Gather(
                input: new List<Gather.InputEnum> { Gather.InputEnum.Dtmf },
                numDigits: 1,
                action: new Uri("/api/Televisit/response", UriKind.Relative)
            );

            gather.Say("You have an appointment. Press 1 for yes, or press 2 for no.");
            response.Append(gather);
            response.Say("We did not receive any input. Goodbye!");
            return Content(response.ToString(), "text/xml");
        }

        // 🟩 Step 3: Handle input (1 or 2)
        [HttpPost("response")]
        public IActionResult ResponseHandler([FromForm] string Digits, [FromForm] string CallSid)
        {
            var response = new VoiceResponse();
            var appointment = _context.Appointments.FirstOrDefault(a => a.CallSid == CallSid);

            if (Digits == "1")
            {
                response.Say("Thank you! Your appointment has been confirmed.");
                if (appointment != null)
                {
                    appointment.Status = "Confirmed";
                    appointment.UserResponse = "1";
                    _context.SaveChanges();
                }
            }
            else if (Digits == "2")
            {
                response.Say("Your appointment has been canceled.");
                if (appointment != null)
                {
                    appointment.Status = "Canceled";
                    appointment.UserResponse = "2";
                    _context.SaveChanges();
                }
            }
            else
            {
                response.Say("Invalid input. Please try again later.");
            }

            return Content(response.ToString(), "text/xml");
        }

        // 🟩 Step 4: View all appointments
        [HttpGet("appointments")]
        public IActionResult GetAppointments()
        {
            var appointments = _context.Appointments
                .OrderByDescending(a => a.Timestamp)
                .ToList();

            return Ok(appointments);
        }
    }
}
