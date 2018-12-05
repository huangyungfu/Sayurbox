# Sayurbox
I am not 100% sure of what is requested in the question so I create web apis for both question

I created in c# wcf because this is the language I am most comfortable with. I didn't create in Java because I will need adjustment and time to learn and I affraid I may not have enough time to both study and do the code after office hours.

The code is created by linking the apis with database (I create a simple dummy database in local sql server)
address:127.0.0.1
username:sa
password:P4ssw0rD
Please attach the mdf in local database first

You will need visual studio in order to test the code. Make sure you run it as administrator. When you run it and the program says wcf cannot find service metadata just click no. The api is ready to use. Just a suggestion I use a program called postman to test whether the api is running or not.

the url for the api are
http://localhost:8732/service1/(the intended link)

the intended link is the url for the intended iperation it is all located in the program, please be sure that you use the appropriate http method and supply the correct json (all service that require json is in array format)

for example:
[
    {
        "OrderId": 1,
        "CustomerId": 1,
        "DriverId": 3
    }
]

for question number 1 I change the input format a bit into
[
    {
        "Id": 1,
        "SayurName": "Apel",
        "Qty": 3,
        "CustomerId": 1,
    },
    {
        "Id": 3,
        "SayurName":"Mangga",
        "Qty": 3,
        "CustomerId": 1,
    }
]

Thank you for your attention
