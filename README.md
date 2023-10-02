# Serilog Mailjet Email Sink

## Download the MailjetEmail NuGet Package
```csharp
Install-Package Serilog.Sinks.MailjetEmail
```

## Create an Instance of the Mailjet Client (Mailjet namespace)
```csharp
var client = new MailjetClient("Your MailJet API Key", "Your MailJet API Secret");
```

## Create an EmailConnectionInfo (Serilog.Sinks.Email namespace)
```csharp
var emailConnectionInfo = new EmailConnectionInfo
{
	EmailSubject = "Application Error",
	FromEmail = "Your From Email",
	ToEmail = "Your To Email",
	MailjetClient = client,
	FromName = "Your Friendly From Name"
};
```

## Create your own customized Serilog logger
```csharp
Log.Logger = new LoggerConfiguration()
	.MinimumLevel.Is(LogEventLevel.Debug)
	.Enrich.WithProcessId()
	.Enrich.WithThreadId()
	.WriteTo.Email(emailConnectionInfo, restrictedToMinimumLevel: LogEventLevel.Error)
	.CreateLogger();
```