# Accessing Static Website using CloudFront

## Prerequisites

- An AWS account where CloudFront and S3 can be hosted.
- S3 bucket should have public access.
- Static WebSite Hosting should be enabled.

## Services Used

- S3 Bucket
- CloudFront

## Things covered

- S3 bucket to store the contents of Static Website.
- Bucket Policy for giving access to OAI(Origin Access Identities).

## Bucket policy template to get S3 objects.
Bucket Policy template : [Template](https://gitlab.com/gokloud-devs/cloudengineering-training/-/blob/shantul/AWS/CloudFront/Shantul/BucketPolicy.txt)

## Steps to create CloudFront distribution

- Access the AWS Console.
- Search for CloudFront service.
- Create a distribution.
	- Enter the origin domain - In our case the name of the bucket (eg - jrastos-cloudfront.s3.us-east-2.amazonaws.com)
	- Select to use the OAI "Yes use OAI (bucket can restrict access to only CloudFront)" 
	- As we are not using SSL certificate so we will stick to HTTP protocol.
	- Hop on to Settings bar and mention the static website page "Default root object" eg - index.html
	- Click on create distribution.
	- Wait for it to be in Deployed state.

## Issue faced while accessing the CloudFront URL.

- Access Denied error
	- Make sure the Bucket policy is assigned on the bucket.
	- Buket should be publicly accessible.
	- Don't forget to mention the "Default root object" else you will have to provide "index.html" after the slash(/index.html).
