using System;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;
using Twilio.Rest.Api.V2010.Account.AvailablePhoneNumberCountry;
using System.Linq;
using System.Collections.Generic;

namespace TwilioCore
{
    class Program
    {
        static void Main(string[] args)
        {
            var accountSid = "ACe7e0c1962bd0b623d06cc2d7a53dbee4";
            var authToken = "dc36a7e1d2ed6dca2f56c36f6c5f2c74";
            var phoneNumber = "15005550006";
            var fromNumber = "15005550006";
            var sms = "Vhere is the money Lebowski??";

            var twilioService = new TwilioService(accountSid, authToken);

            Console.WriteLine($"Twilio Account SID: {twilioService.AccountSid}");
            Console.WriteLine($"Twilio Authorization Token: {twilioService.AuthToken}");

            Console.WriteLine("Creating phone number...");
            Console.WriteLine();
            var number = twilioService.CreateNumber(phoneNumber);

            if (number != null)
            {
                Console.WriteLine($"SID: {number.Sid}");
                Console.WriteLine($"Date Created: {number.DateCreated}");
                Console.WriteLine($"Date Updated: {number.DateUpdated}");
                Console.WriteLine($"Friendly Name: {number.FriendlyName}");
                Console.WriteLine($"Account Sid: {number.AccountSid}");
                Console.WriteLine($"Phone Number: {number.PhoneNumber}");
                Console.WriteLine($"API Version: {number.ApiVersion}");
                Console.WriteLine($"Voice Caller ID Lookup: {number.VoiceCallerIdLookup}");
                Console.WriteLine($"Voice URL: {number.VoiceUrl}");
                Console.WriteLine($"Voice Method: {number.VoiceMethod}");
                Console.WriteLine($"Voice Fallback Url: {number.VoiceFallbackUrl}");
                Console.WriteLine($"Voice Fallback Method: {number.VoiceFallbackMethod}");
                Console.WriteLine($"Status Callback: {number.StatusCallback}");
                Console.WriteLine($"Status Callback Method: {number.StatusCallbackMethod}");
                Console.WriteLine($"Voice Application Sid: {number.VoiceApplicationSid}");
                Console.WriteLine($"Trunk Sid: {number.TrunkSid}");
                Console.WriteLine($"SMS URL: {number.SmsUrl}");
                Console.WriteLine($"SMS Method: {number.SmsMethod}");
                Console.WriteLine($"Date Created: {number.DateCreated}");
                Console.WriteLine();
            }
            
            Console.WriteLine("Creating message...");
            Console.WriteLine();
            twilioService.CreateMessage(phoneNumber, fromNumber, sms);

            twilioService.GetMessages();

            Console.ReadKey();
        }
    }

    public class TwilioService
    {
        private readonly string _accountSid;
        private readonly string _authToken;

        public TwilioService(string accountSid, string authToken)
        {
            _accountSid = accountSid;
            _authToken = authToken;

            TwilioClient.Init(_accountSid, _authToken);
        }

        public string AccountSid
        {
            get { return _accountSid; }
        }

        public string AuthToken
        {
            get { return _authToken; }
        }

        public IncomingPhoneNumberResource CreateNumber(string phoneNumber)
        {
            try
            {
				IncomingPhoneNumberResource number = IncomingPhoneNumberResource.Create(
					phoneNumber: new PhoneNumber($"+{phoneNumber}"),
				    voiceUrl: new Uri("http://demo.twilio.com/docs/voice.xml"),
                    smsUrl: new Uri("http://demo.twilio.com/test/url/voice.xml"));

                return number;
            }
            catch (Twilio.Exceptions.ApiException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            return null;
        }

        public void CreateMessage(string number, string fromNumber, string sms)
        {
            try
            {
                var to = new PhoneNumber($"+{number}");
                var message = MessageResource.Create(
                    to,
                    from: new PhoneNumber($"+{fromNumber}"),
                    body: sms);

                    Console.WriteLine($"Message SID: {message.Sid}");
                    Console.WriteLine($"Message: {message.Body}");
            }
            catch (Twilio.Exceptions.ApiException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        public IEnumerable<LocalResource> AvailableNumbers()
        {
            var localAvailableNumbers = LocalResource.Read("US", areaCode: 510);

            foreach(var number in localAvailableNumbers)
            {
                Console.WriteLine($"Available number: {number}");
                break;
            }

            return localAvailableNumbers;
        }

        public void GetMessages()
        {
            var messages = MessageResource.Read();

            foreach (var message in messages)
            {
                Console.WriteLine($"Reading message...: {message.Body}");
            }
        }
    }
}
