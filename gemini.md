INPUT:
{
  "Email": "goaintern012025@gmail.com",
  "MobileNumber": "1234567890"
}







Output:
{
  "statusCode": 500,
  "headers": {
    "Content-Type": "application/json"
  },
  "body": "{\"error\":\"Failed to process request.\"}",
  "isBase64Encoded": false
}

LOGS:

2025-08-13T10:59:18.409Z
INIT_START Runtime Version: dotnet:8.v47	Runtime Version ARN: arn:aws:lambda:ap-south-1::runtime:7c4f52ff2ae949a1b7a3f3b2dc97ef8b9570683404a7afd8c7cd093822c1a526

INIT_START Runtime Version: dotnet:8.v47 Runtime Version ARN: arn:aws:lambda:ap-south-1::runtime:7c4f52ff2ae949a1b7a3f3b2dc97ef8b9570683404a7afd8c7cd093822c1a526
2025-08-13T10:59:18.797Z
START RequestId: aaa23c36-9baf-4210-b589-f2bad3648ed6 Version: $LATEST

START RequestId: aaa23c36-9baf-4210-b589-f2bad3648ed6 Version: $LATEST
2025-08-13T10:59:21.473Z
2025-08-13T10:59:21.433Z	aaa23c36-9baf-4210-b589-f2bad3648ed6	fail	Amazon.DynamoDBv2.AmazonDynamoDBException: The table does not have the specified index: EmailIndex
 ---> Amazon.Runtime.Internal.HttpErrorResponseException: Exception of type 'Amazon.Runtime.Internal.HttpErrorResponseException' was thrown.
   at Amazon.Runtime.HttpWebRequestMessage.ProcessHttpResponseMessage(HttpResponseMessage responseMessage)
   at Amazon.Runtime.HttpWebRequestMessage.GetResponseAsync(CancellationToken cancellationToken)
   at Amazon.Runtime.Internal.HttpHandler`1.InvokeAsync[T](IExecutionContext executionContext)
   at Amazon.Runtime.Internal.Unmarshaller.InvokeAsync[T](IExecutionContext executionContext)
   at Amazon.Runtime.Internal.ErrorHandler.InvokeAsync[T](IExecutionContext executionContext)
   --- End of inner exception stack trace ---
   at Amazon.Runtime.Internal.HttpErrorResponseExceptionHandler.HandleExceptionStream(IRequestContext requestContext, IWebResponseData httpErrorResponse, HttpErrorResponseException exception, Stream responseStream)
   at Amazon.Runtime.Internal.HttpErrorResponseExceptionHandler.HandleExceptionAsync(IExecutionContext executionContext, HttpErrorResponseException exception)
   at Amazon.Runtime.Internal.ExceptionHandler`1.HandleAsync(IExecutionContext executionContext, Exception exception)
   at Amazon.Runtime.Internal.ErrorHandler.ProcessExceptionAsync(IExecutionContext executionContext, Exception exception)
   at Amazon.Runtime.Internal.ErrorHandler.InvokeAsync[T](IExecutionContext executionContext)
   at Amazon.Runtime.Internal.CallbackHandler.InvokeAsync[T](IExecutionContext executionContext)
   at Amazon.Runtime.Internal.Signer.InvokeAsync[T](IExecutionContext executionContext)
   at Amazon.Runtime.Internal.EndpointDiscoveryHandler.InvokeAsync[T](IExecutionContext executionContext)
   at Amazon.Runtime.Internal.EndpointDiscoveryHandler.InvokeAsync[T](IExecutionContext executionContext)
   at Amazon.Runtime.Internal.RetryHandler.InvokeAsync[T](IExecutionContext executionContext)
   at Amazon.Runtime.Internal.RetryHandler.InvokeAsync[T](IExecutionContext executionContext)
   at Amazon.Runtime.Internal.CallbackHandler.InvokeAsync[T](IExecutionContext executionContext)
   at Amazon.Runtime.Internal.BaseAuthResolverHandler.InvokeAsync[T](IExecutionContext executionContext)
   at Amazon.Runtime.Internal.CallbackHandler.InvokeAsync[T](IExecutionContext executionContext)
   at Amazon.Runtime.Internal.ErrorCallbackHandler.InvokeAsync[T](IExecutionContext executionContext)
   at Amazon.Runtime.Internal.MetricsHandler.InvokeAsync[T](IExecutionContext executionContext)
   at Shared.DynamoDbHelper.GetUserByEmailAsync(String email) in D:\workspace\SESLambda\OtpSystem\Shared\DynamoDbHelper.cs:line 18
   at Shared.DynamoDbHelper.InitUserIfMissingAsync(String email, String mobile) in D:\workspace\SESLambda\OtpSystem\Shared\DynamoDbHelper.cs:line 84
   at SaveOrResendOtpLambda.Function.FunctionHandler(SaveOrResendRequest payload, ILambdaContext context) in D:\workspace\SESLambda\OtpSystem\SaveOrResendOtpLambda\Function.cs:line 36

2025-08-13T10:59:21.433Z aaa23c36-9baf-4210-b589-f2bad3648ed6 fail Amazon.DynamoDBv2.AmazonDynamoDBException: The table does not have the specified index: EmailIndex ---> Amazon.Runtime.Internal.HttpErrorResponseException: Exception of type 'Amazon.Runtime.Internal.HttpErrorResponseException' was thrown. at Amazon.Runtime.HttpWebRequestMessage.ProcessHttpResponseMessage(HttpResponseMessage responseMessage) at Amazon.Runtime.HttpWebRequestMessage.GetResponseAsync(CancellationToken cancellationToken) at Amazon.Runtime.Internal.HttpHandler`1.InvokeAsync[T](IExecutionContext executionContext) at Amazon.Runtime.Internal.Unmarshaller.InvokeAsync[T](IExecutionContext executionContext) at Amazon.Runtime.Internal.ErrorHandler.InvokeAsync[T](IExecutionContext executionContext) --- End of inner exception stack trace --- at Amazon.Runtime.Internal.HttpErrorResponseExceptionHandler.HandleExceptionStream(IRequestContext requestContext, IWebResponseData httpErrorResponse, HttpErrorResponseException exception, Stream responseStream) at Amazon.Runtime.Internal.HttpErrorResponseExceptionHandler.HandleExceptionAsync(IExecutionContext executionContext, HttpErrorResponseException exception) at Amazon.Runtime.Internal.ExceptionHandler`1.HandleAsync(IExecutionContext executionContext, Exception exception) at Amazon.Runtime.Internal.ErrorHandler.ProcessExceptionAsync(IExecutionContext executionContext, Exception exception) at Amazon.Runtime.Internal.ErrorHandler.InvokeAsync[T](IExecutionContext executionContext) at Amazon.Runtime.Internal.CallbackHandler.InvokeAsync[T](IExecutionContext executionContext) at Amazon.Runtime.Internal.Signer.InvokeAsync[T](IExecutionContext executionContext) at Amazon.Runtime.Internal.EndpointDiscoveryHandler.InvokeAsync[T](IExecutionContext executionContext) at Amazon.Runtime.Internal.EndpointDiscoveryHandler.InvokeAsync[T](IExecutionContext executionContext) at Amazon.Runtime.Internal.RetryHandler.InvokeAsync[T](IExecutionContext executionContext) at Amazon.Runtime.Internal.RetryHandler.InvokeAsync[T](IExecutionContext executionContext) at Amazon.Runtime.Internal.CallbackHandler.InvokeAsync[T](IExecutionContext executionContext) at Amazon.Runtime.Internal.BaseAuthResolverHandler.InvokeAsync[T](IExecutionContext executionContext) at Amazon.Runtime.Internal.CallbackHandler.InvokeAsync[T](IExecutionContext executionContext) at Amazon.Runtime.Internal.ErrorCallbackHandler.InvokeAsync[T](IExecutionContext executionContext) at Amazon.Runtime.Internal.MetricsHandler.InvokeAsync[T](IExecutionContext executionContext) at Shared.DynamoDbHelper.GetUserByEmailAsync(String email) in D:\workspace\SESLambda\OtpSystem\Shared\DynamoDbHelper.cs:line 18 at Shared.DynamoDbHelper.InitUserIfMissingAsync(String email, String mobile) in D:\workspace\SESLambda\OtpSystem\Shared\DynamoDbHelper.cs:line 84 at SaveOrResendOtpLambda.Function.FunctionHandler(SaveOrResendRequest payload, ILambdaContext context) in D:\workspace\SESLambda\OtpSystem\SaveOrResendOtpLambda\Function.cs:line 36
2025-08-13T10:59:21.892Z
END RequestId: aaa23c36-9baf-4210-b589-f2bad3648ed6

END RequestId: aaa23c36-9baf-4210-b589-f2bad3648ed6
2025-08-13T10:59:21.892Z
REPORT RequestId: aaa23c36-9baf-4210-b589-f2bad3648ed6	Duration: 3093.08 ms	Billed Duration: 3094 ms	Memory Size: 256 MB	Max Memory Used: 100 MB	Init Duration: 384.96 ms	

REPORT RequestId: aaa23c36-9baf-4210-b589-f2bad3648ed6 Duration: 3093.08 ms Billed Duration: 3094 ms Memory Size: 256 MB Max Memory Used: 100 MB Init Duration: 384.96 ms
2025-08-13T11:00:21.008Z
START RequestId: 2331460b-e67a-42bf-b305-c07bd2a5ecd6 Version: $LATEST

START RequestId: 2331460b-e67a-42bf-b305-c07bd2a5ecd6 Version: $LATEST
2025-08-13T11:00:21.051Z
2025-08-13T11:00:21.051Z	2331460b-e67a-42bf-b305-c07bd2a5ecd6	fail	Amazon.DynamoDBv2.AmazonDynamoDBException: The table does not have the specified index: EmailIndex
 ---> Amazon.Runtime.Internal.HttpErrorResponseException: Exception of type 'Amazon.Runtime.Internal.HttpErrorResponseException' was thrown.
   at Amazon.Runtime.HttpWebRequestMessage.ProcessHttpResponseMessage(HttpResponseMessage responseMessage)
   at Amazon.Runtime.HttpWebRequestMessage.GetResponseAsync(CancellationToken cancellationToken)
   at Amazon.Runtime.Internal.HttpHandler`1.InvokeAsync[T](IExecutionContext executionContext)
   at Amazon.Runtime.Internal.Unmarshaller.InvokeAsync[T](IExecutionContext executionContext)
   at Amazon.Runtime.Internal.ErrorHandler.InvokeAsync[T](IExecutionContext executionContext)
   --- End of inner exception stack trace ---
   at Amazon.Runtime.Internal.HttpErrorResponseExceptionHandler.HandleExceptionStream(IRequestContext requestContext, IWebResponseData httpErrorResponse, HttpErrorResponseException exception, Stream responseStream)
   at Amazon.Runtime.Internal.HttpErrorResponseExceptionHandler.HandleExceptionAsync(IExecutionContext executionContext, HttpErrorResponseException exception)
   at Amazon.Runtime.Internal.ExceptionHandler`1.HandleAsync(IExecutionContext executionContext, Exception exception)
   at Amazon.Runtime.Internal.ErrorHandler.ProcessExceptionAsync(IExecutionContext executionContext, Exception exception)
   at Amazon.Runtime.Internal.ErrorHandler.InvokeAsync[T](IExecutionContext executionContext)
   at Amazon.Runtime.Internal.CallbackHandler.InvokeAsync[T](IExecutionContext executionContext)
   at Amazon.Runtime.Internal.Signer.InvokeAsync[T](IExecutionContext executionContext)
   at Amazon.Runtime.Internal.EndpointDiscoveryHandler.InvokeAsync[T](IExecutionContext executionContext)
   at Amazon.Runtime.Internal.EndpointDiscoveryHandler.InvokeAsync[T](IExecutionContext executionContext)
   at Amazon.Runtime.Internal.RetryHandler.InvokeAsync[T](IExecutionContext executionContext)
   at Amazon.Runtime.Internal.RetryHandler.InvokeAsync[T](IExecutionContext executionContext)
   at Amazon.Runtime.Internal.CallbackHandler.InvokeAsync[T](IExecutionContext executionContext)
   at Amazon.Runtime.Internal.BaseAuthResolverHandler.InvokeAsync[T](IExecutionContext executionContext)
   at Amazon.Runtime.Internal.CallbackHandler.InvokeAsync[T](IExecutionContext executionContext)
   at Amazon.Runtime.Internal.ErrorCallbackHandler.InvokeAsync[T](IExecutionContext executionContext)
   at Amazon.Runtime.Internal.MetricsHandler.InvokeAsync[T](IExecutionContext executionContext)
   at Shared.DynamoDbHelper.GetUserByEmailAsync(String email) in D:\workspace\SESLambda\OtpSystem\Shared\DynamoDbHelper.cs:line 18
   at Shared.DynamoDbHelper.InitUserIfMissingAsync(String email, String mobile) in D:\workspace\SESLambda\OtpSystem\Shared\DynamoDbHelper.cs:line 84
   at SaveOrResendOtpLambda.Function.FunctionHandler(SaveOrResendRequest payload, ILambdaContext context) in D:\workspace\SESLambda\OtpSystem\SaveOrResendOtpLambda\Function.cs:line 36

2025-08-13T11:00:21.051Z 2331460b-e67a-42bf-b305-c07bd2a5ecd6 fail Amazon.DynamoDBv2.AmazonDynamoDBException: The table does not have the specified index: EmailIndex ---> Amazon.Runtime.Internal.HttpErrorResponseException: Exception of type 'Amazon.Runtime.Internal.HttpErrorResponseException' was thrown. at Amazon.Runtime.HttpWebRequestMessage.ProcessHttpResponseMessage(HttpResponseMessage responseMessage) at Amazon.Runtime.HttpWebRequestMessage.GetResponseAsync(CancellationToken cancellationToken) at Amazon.Runtime.Internal.HttpHandler`1.InvokeAsync[T](IExecutionContext executionContext) at Amazon.Runtime.Internal.Unmarshaller.InvokeAsync[T](IExecutionContext executionContext) at Amazon.Runtime.Internal.ErrorHandler.InvokeAsync[T](IExecutionContext executionContext) --- End of inner exception stack trace --- at Amazon.Runtime.Internal.HttpErrorResponseExceptionHandler.HandleExceptionStream(IRequestContext requestContext, IWebResponseData httpErrorResponse, HttpErrorResponseException exception, Stream responseStream) at Amazon.Runtime.Internal.HttpErrorResponseExceptionHandler.HandleExceptionAsync(IExecutionContext executionContext, HttpErrorResponseException exception) at Amazon.Runtime.Internal.ExceptionHandler`1.HandleAsync(IExecutionContext executionContext, Exception exception) at Amazon.Runtime.Internal.ErrorHandler.ProcessExceptionAsync(IExecutionContext executionContext, Exception exception) at Amazon.Runtime.Internal.ErrorHandler.InvokeAsync[T](IExecutionContext executionContext) at Amazon.Runtime.Internal.CallbackHandler.InvokeAsync[T](IExecutionContext executionContext) at Amazon.Runtime.Internal.Signer.InvokeAsync[T](IExecutionContext executionContext) at Amazon.Runtime.Internal.EndpointDiscoveryHandler.InvokeAsync[T](IExecutionContext executionContext) at Amazon.Runtime.Internal.EndpointDiscoveryHandler.InvokeAsync[T](IExecutionContext executionContext) at Amazon.Runtime.Internal.RetryHandler.InvokeAsync[T](IExecutionContext executionContext) at Amazon.Runtime.Internal.RetryHandler.InvokeAsync[T](IExecutionContext executionContext) at Amazon.Runtime.Internal.CallbackHandler.InvokeAsync[T](IExecutionContext executionContext) at Amazon.Runtime.Internal.BaseAuthResolverHandler.InvokeAsync[T](IExecutionContext executionContext) at Amazon.Runtime.Internal.CallbackHandler.InvokeAsync[T](IExecutionContext executionContext) at Amazon.Runtime.Internal.ErrorCallbackHandler.InvokeAsync[T](IExecutionContext executionContext) at Amazon.Runtime.Internal.MetricsHandler.InvokeAsync[T](IExecutionContext executionContext) at Shared.DynamoDbHelper.GetUserByEmailAsync(String email) in D:\workspace\SESLambda\OtpSystem\Shared\DynamoDbHelper.cs:line 18 at Shared.DynamoDbHelper.InitUserIfMissingAsync(String email, String mobile) in D:\workspace\SESLambda\OtpSystem\Shared\DynamoDbHelper.cs:line 84 at SaveOrResendOtpLambda.Function.FunctionHandler(SaveOrResendRequest payload, ILambdaContext context) in D:\workspace\SESLambda\OtpSystem\SaveOrResendOtpLambda\Function.cs:line 36
2025-08-13T11:00:21.073Z
END RequestId: 2331460b-e67a-42bf-b305-c07bd2a5ecd6

END RequestId: 2331460b-e67a-42bf-b305-c07bd2a5ecd6
2025-08-13T11:00:21.073Z
REPORT RequestId: 2331460b-e67a-42bf-b305-c07bd2a5ecd6	Duration: 63.37 ms	Billed Duration: 64 ms	Memory Size: 256 MB	Max Memory Used: 101 MB	

REPORT RequestId: 2331460b-e67a-42bf-b305-c07bd2a5ecd6 Duration: 63.37 ms Billed Duration: 64 ms Memory Size: 256 MB Max Memory Used: 101 MB
No newer events at this moment. 
Auto retry paused.
 
Resume
 