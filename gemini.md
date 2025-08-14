Hi
I am making a project in aws using a lambda function: SaveOrResendOtpLambda
I have a dynamoDB tables: DATA.UserAccess
I am using .net8




Main Task: So my code is working proeprly as expected but I have to change some fields which will be saved in dynamoDB

Some tasks are:
Properly saving createDate
Auto increment ID and start from 0001
Removing the columns form DATA.userAccess tables- varificationCode, varificationCodeExpiry
Email should not come in spam 
Connect to frontend and test 

Table name: DATA.userAccess
The fields are:
Id (Number) auto Increment starts from 0001
email
mobile number
IsValidated - Boolean flag indicating whether the user has successfully validated (1 = Yes, 0 = No).
VerificationCode
VerificationCodeExpiry
VerifiationAttempt - Count of how many times the user has attempted to verify the code.
createData - Timestamp when the access record was created.
updateDate - Timestamp when the record was last updated (e.g., after a retry or code resend).




table DATA.UserAccess
-----------------------------------------
Name: EmailIndex
Status: Active
Partition key: Email (String)
Sort key: - 
Read capacity: On-demand
Write capacity:On-demand
Projected attributes: All
Size: 583 bytes
Item count: 4
-----------------------------------------

no need to send any data to other table: OTPVerification
whatever OTP is sent to mail must be stored in the table DATA.userAccess
also if the user is alredy verified in the DATA.userAccess table then no need to verify again
