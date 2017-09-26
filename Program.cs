using System;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;
using Twilio.Rest.Api.V2010.Account.AvailablePhoneNumberCountry;
using System.Linq;
using System.Collections.Generic;
using Twilio.Rest.Api.V2010.Account.Usage.Record;

namespace TwilioCore
{
    class Program
    {
        static void Main(string[] args)
        {
            var accountSid = "account_sid_here";
            var authToken = "auth_token_here";

            //var phoneNumber = "15005550006";
            //var fromNumber = "15005550006";
            //var sms = "Vhere is the money Lebowski??";

            var twilioService = new TwilioService(accountSid, authToken);

            Console.WriteLine($"Twilio Account SID: {twilioService.AccountSid}");
            Console.WriteLine($"Twilio Authorization Token: {twilioService.AuthToken}");

            //Console.WriteLine("Creating phone number...\r\n");
            //var incomingNumberResource = twilioService.CreateNumber(phoneNumber);

            //if (fromNumber != null)
            //{
            //    twilioService.ReadIncomingPhoneNumberResource(incomingNumberResource);    
            //}
            
            //Console.WriteLine("Creating message...\r\n");
            //twilioService.CreateMessage(phoneNumber, fromNumber, sms);

            //twilioService.GetMessages();

            var countryCode = "US";
            var areaCode = 918;

            var localResources = twilioService.GetAvailableNumbers(countryCode, areaCode);
            twilioService.ReadLocalResources(localResources);

            twilioService.GetMobileNumbers();

            //var callbackUrlString = "http://www.example.com/";
            //var triggerSid = twilioService.CreateTrigger(callbackUrlString);

            var allTimeResources = twilioService.Usage();
            twilioService.ReadUsages(allTimeResources);
            
            var incomingPhoneNumberResources = twilioService.GetIncomingPhoneNumberResources();
            twilioService.ReadIncomingPhoneNumberResources(incomingPhoneNumberResources);

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

        public IEnumerable<LocalResource> GetAvailableNumbers(string countryCode, int areaCode)
        {
            return LocalResource.Read(countryCode, areaCode: areaCode, mmsEnabled: true);
        }

        public void ReadLocalResources(IEnumerable<LocalResource> localResources)
        {
            Console.WriteLine($"Available numbers: {localResources.Count()}");

			foreach (var localResource in localResources)
			{
				Console.WriteLine($"\r\nFriendly Name: {localResource.FriendlyName}");
				Console.WriteLine($"Phone Number: {localResource.PhoneNumber}");
                Console.WriteLine($"Locality: {localResource.Locality}");
				Console.WriteLine($"Rate Center: {localResource.RateCenter}");
				Console.WriteLine($"State : {localResource.Region}");
				Console.WriteLine($"Postal Code: {localResource.PostalCode}");
                Console.WriteLine($"MMS Enables: {localResource.Capabilities.Mms}\r\n");
			}
        }

        public void GetMobileNumbers()
        {
            try
            {
                IEnumerable<MobileResource> mobileResources = MobileResource.Read("US", areaCode: 918);
            }
            catch (Twilio.Exceptions.ApiException ex)
            {
                Console.WriteLine($"ERROR: {ex.Message}");
            }
        }

        public void GetMessages()
        {
            var messages = MessageResource.Read();

            foreach (var message in messages)
            {
                Console.WriteLine($"Reading message...: {message.Body}");
            }
        }

        public void Triggers()
        {
            var temp = Twilio.Rest.Api.V2010.Account.Usage.TriggerResource.Read();
        }

        public string CreateTrigger(string callbackUrlString)
        {
			var callbackUrl = new Uri(callbackUrlString);
			const string triggerValue = "1000";
			var trigger = Twilio.Rest.Api.V2010.Account.Usage.TriggerResource.Create(callbackUrl,
												 triggerValue,
												 Twilio.Rest.Api.V2010.Account.Usage.TriggerResource.UsageCategoryEnum.Sms);

            return trigger.Sid;
        }

        public IEnumerable<AllTimeResource> Usage()
        {
            return AllTimeResource.Read();
        }

        public void ReadUsages(IEnumerable<AllTimeResource> resources)
        {
            Console.WriteLine($"\r\nAll Time Resources: {resources.Count()}\r\n");

            foreach (var resource in resources)
            {
				if (resource.Category == AllTimeResource.CategoryEnum.Calls
					|| resource.Category == AllTimeResource.CategoryEnum.CallsInboundLocal
					|| resource.Category == AllTimeResource.CategoryEnum.SmsInboundLongcode
					|| resource.Category == AllTimeResource.CategoryEnum.Sms
					|| resource.Category == AllTimeResource.CategoryEnum.Totalprice
					|| resource.Category == AllTimeResource.CategoryEnum.PhonenumbersLocal
					|| resource.Category == AllTimeResource.CategoryEnum.Phonenumbers
					|| resource.Category == AllTimeResource.CategoryEnum.CallsInbound
					|| resource.Category == AllTimeResource.CategoryEnum.SmsInbound
					|| resource.Category == AllTimeResource.CategoryEnum.SmsOutbound
					|| resource.Category == AllTimeResource.CategoryEnum.SmsOutboundLongcode)
				{
					Console.WriteLine($"\r\nDescription: {resource.Description}");
					Console.WriteLine($"Category: {resource.Category}");
					Console.WriteLine($"Count: {resource.Count}");
					Console.WriteLine($"Price: {resource.Price}");
					Console.WriteLine($"Usage: {resource.Usage}\r\n");
				}
            }
        }

        public IEnumerable<IncomingPhoneNumberResource> GetIncomingPhoneNumberResources()
        {
            return IncomingPhoneNumberResource.Read();
        }

        public void ReadIncomingPhoneNumberResources(IEnumerable<IncomingPhoneNumberResource> resources)
        {
            Console.WriteLine($"\r\nTotal Incoming Phone Number Resources: {resources.Count()}\r\n");

            foreach (var resource in resources)
            {
                ReadIncomingPhoneNumberResource(resource);
            }
        }

        public void ReadIncomingPhoneNumberResource(IncomingPhoneNumberResource resource)
        {
			Console.WriteLine($"\r\nAccount SID: {resource.AccountSid}");
			Console.WriteLine($"SID: {resource.Sid}");
			Console.WriteLine($"Friendly Name: {resource.FriendlyName}");
			Console.WriteLine($"Phone Number: {resource.PhoneNumber}\r\n");
        }
    }
}
