using Serilog.Sinks.PeriodicBatching;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Serilog.Events;
using Serilog.Formatting;
using Mailjet.Client.TransactionalEmails;
using Mailjet.Client;

namespace Serilog.Sinks.Email
{
	class MailJetEmailSink : IBatchedLogEventSink
	{
        readonly EmailConnectionInfo _connectionInfo;

		readonly MailjetClient _client;

		readonly ITextFormatter _textFormatter;


        /// <summary>
        /// Construct a sink emailing with the specified details.
		/// </summary>
		/// <param name="connectionInfo">Connection information used to construct the SMTP client and mail messages.</param>
		/// <param name="textFormatter">Supplies culture-specific formatting information, or null.</param>
		public MailJetEmailSink(EmailConnectionInfo connectionInfo, ITextFormatter textFormatter)
        {
			_connectionInfo = connectionInfo ?? throw new ArgumentNullException(nameof(connectionInfo));
			_textFormatter = textFormatter;
			_client = connectionInfo.MailjetClient;
		}

		/// <summary>
		/// Emit a batch of log events, running asynchronously.
		/// </summary>
		/// <param name="events">The events to emit.</param>
		/// <remarks>Override either <see cref="PeriodicBatchingSink.EmitBatch"/> or <see cref="PeriodicBatchingSink.EmitBatchAsync"/>,
		/// not both.</remarks>
		public async Task EmitBatchAsync(IEnumerable<LogEvent> events)
		{
			if (events == null)
                throw new ArgumentNullException(nameof(events));

            var payload = new StringWriter();

            var eventsSet = events.ToList();
            foreach (var logEvent in eventsSet)
            {
                _textFormatter.Format(logEvent, payload);
            }

            var email = new TransactionalEmailBuilder()
                .WithFrom(new SendContact(_connectionInfo.FromEmail, _connectionInfo.FromName))
                .WithSubject(_connectionInfo.EmailSubject)
                .WithTextPart(payload.ToString())
                .WithTo(new SendContact(_connectionInfo.ToEmail))
                .Build();

            await _client.SendTransactionalEmailAsync(email);
        }

        public Task OnEmptyBatchAsync()
        {
			return Task.FromResult(0);
        }
	}
}
