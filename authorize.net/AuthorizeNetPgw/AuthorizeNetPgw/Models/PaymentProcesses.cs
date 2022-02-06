using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AuthorizeNet.Api.Contracts.V1;
using AuthorizeNet.Api.Controllers;
using AuthorizeNet.Api.Controllers.Bases;

namespace AuthorizeNetPgw.Models
{
    public class PaymentProcesses
    {
        const string apiLoginId = "2eF5NV5yL";
        const string transactionKey = "5Y4z33JU284PjEc4";
        public TransactionResponse ChargeCredit(PaymentModel payment)
        {
            ApiOperationBase<ANetApiRequest, ANetApiResponse>.RunEnvironment = AuthorizeNet.Environment.SANDBOX;
            ApiOperationBase<ANetApiRequest, ANetApiResponse>.MerchantAuthentication = new merchantAuthenticationType()
            {
                name = apiLoginId,
                ItemElementName = ItemChoiceType.transactionKey,
                Item = transactionKey,
            };
            var creditCard = new creditCardType
            {
                cardNumber = payment.CardNumber,//"Ex: 4111111111111111",
                expirationDate = payment.Month + payment.Year,//"Ex: 0522"
                cardCode = payment.CardCode,//"Ex: 111"
            };
            var billingAddress = new customerAddressType
            {
                firstName = payment.FirstName,
                lastName = payment.LastName,
                city = payment.Address1,
                address = payment.Address2,
                zip = payment.Postcode,
                phoneNumber = payment.Phone
            };
            var paymentType = new paymentType { Item = creditCard };
            var lineItems = new lineItemType[2];
            lineItems[0] = new lineItemType { itemId = "1", name = "t-shirt", quantity = 2, unitPrice = new Decimal(1.00) };
            lineItems[1] = new lineItemType { itemId = "2", name = "snowboard", quantity = 1, unitPrice = new Decimal(1.00) };

            var transactionRequest = new transactionRequestType
            {
                transactionType = transactionTypeEnum.authCaptureTransaction.ToString(),    // charge the card
                amount = payment.Amount,
                payment = paymentType,
                billTo = billingAddress,
                lineItems = lineItems
            };
            var request = new createTransactionRequest { transactionRequest = transactionRequest };
            var controller = new createTransactionController(request);
            controller.Execute();
            var resCode = controller.GetResultCode();
            var resAll = controller.GetResults();
            var response = controller.GetApiResponse();
            TransactionResponse result = new TransactionResponse();
            if (response != null)
            {
                result.resultCode = response.messages.resultCode;
                if (response.messages.resultCode == messageTypeEnum.Ok)
                {
                    if (response.transactionResponse.messages != null)
                    {
                        result.transId = response.transactionResponse.transId;
                        result.responseCode = response.transactionResponse.responseCode;
                        result.messageCode = response.transactionResponse.messages[0].code;
                        result.description = response.transactionResponse.messages[0].description;
                        result.authCode = response.transactionResponse.authCode;
                    }
                    else
                    {
                        if (response.transactionResponse.errors != null)
                        {
                            result.errorCode = response.transactionResponse.errors[0].errorCode;
                            result.errorText = response.transactionResponse.errors[0].errorText;
                        }
                    }
                }
                else
                {
                    if (response.transactionResponse != null && response.transactionResponse.errors != null)
                    {
                        result.errorCode = response.transactionResponse.errors[0].errorCode;
                        result.errorText = response.transactionResponse.errors[0].errorText;
                    }
                    else
                    {
                        result.errorCode = response.messages.message[0].code;
                        result.errorText = response.messages.message[0].text;
                    }
                }
            }
            else
            {
                result.errorCode = "NONE";
                result.errorText = "Failed Transaction,, Unkown Error";
            }
            return result;
        }
    }
}