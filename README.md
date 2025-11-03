# TelevisitAPI

TelevisitAPI is a **.NET 8 Web API** that automates appointment confirmation calls using **Twilio**.
It plays a message — “You have an appointment. Press 1 for Yes, 2 for No.” — and records the patient’s response, storing the data in a **SQLite database** for tracking and analytics.

---

## 🚀 Features

* 📞 Twilio call integration to notify users of appointments
* 🧠 Interactive voice response (IVR) with keypad input (1 = Yes, 2 = No)
* 💾 SQLite database with Entity Framework Core for persistence
* 🧰 Swagger UI for testing and documentation
* 🔐 Configurable API via `appsettings.json`

---

## 🧩 Technologies Used

* **.NET 8 / ASP.NET Core Web API**
* **Entity Framework Core**
* **SQLite**
* **Twilio API**
* **Swagger / Swashbuckle**

---

## ⚙️ Setup Instructions

### 1. Clone the Repository

```bash
git clone https://github.com/<your-username>/TelevisitAPI.git
cd TelevisitAPI
```

### 2. Install Dependencies

Make sure you have:

* .NET 8 SDK installed
* SQLite (optional, built-in provider used by EF Core)
* Twilio account credentials

### 3. Configure Environment Variables

In `appsettings.json`, update:

```json
"Twilio": {
  "AccountSid": "YOUR_TWILIO_ACCOUNT_SID",
  "AuthToken": "YOUR_TWILIO_AUTH_TOKEN",
  "FromNumber": "+1234567890"
},
"ConnectionStrings": {
  "DefaultConnection": "Data Source=appointment.db"
}
```

### 4. Run the API

```bash
dotnet run
```

Then open:

```
https://localhost:7084/swagger
```

---

## 📦 Database

The app automatically creates the `appointment.db` SQLite file at startup.
You can view the data using:

* Visual Studio → SQL Server Object Explorer → Add Connection → SQLite → appointment.db
* Or via `DB Browser for SQLite`.

---

## 🧠 Future Enhancements

* Add dashboard for appointment analytics
* Enable SMS confirmation flow
* Integrate with EHR/EMR scheduling systems

---

## 🧑‍💻 Author

**Om Upadhyay**
🎓 4th-year Computer Science Student
💼 Focus: Cloud Data Engineering | Computer Vision | Full-Stack Development
