# sti_sms
An SMS/  zipcode based STI clinic locator

This application uses a CDC web service to locate STI test clinics based on zipcode and return the top 3 results via SMS.

It uses Twilio for incoming and outgoing SMS communications.

Setup:
* Copy Web.config.example and remove the .example to create your empty Web.config
* Create Twilio account and get an SMS number
* Add Twilio credentials and number to the Web.config
* Deploy the web application to a public address where Twilio can reach it
* In Twilio administration, add the url for the deployed web application to the Twilio SMS number Request Url field

Usage: 
* Text a five digit zipcode to the SMS number registered
* You should SMS responses for three clinics in your area, with name, address, and phone number
