# Accessing Static Website using CloudFront

## Prerequisites :-

- An AWS account where CloudFront and S3 can be hosted.
- S3 bucket should have public access.
- Static WebSite Hosting should be enabled.

## Services Used :-

### S3 Bucket
![S3_Bucket](/uploads/1fcd1966d49ece8952ac35dbbc75f01e/S3_Bucket.PNG)

### CloudFront
![CloudFront_Distribution](/uploads/e81723c06dda3ad16573a6e0519ecd2c/CloudFront_Distribution.PNG)

## Things covered :-

- Providing public access to S3
- S3 bucket to store the contents of Static Website.
- Bucket Policy for giving access to OAI(Origin Access Identities).

## Bucket policy to get S3 objects :-

```
{
    "Version": "2008-10-17",
    "Id": "PolicyForCloudFrontPrivateContent",
    "Statement": [
        {
            "Sid": "1",
            "Effect": "Allow",
            "Principal": {
                "AWS": "arn:aws:iam::cloudfront:user/CloudFront Origin Access Identity E3L5D0I5RO6217"
            },
            "Action": "s3:GetObject",
            "Resource": "arn:aws:s3:::tikufinalcf/*"
        }
    ]
}

```

## Steps to create CloudFront distribution :-

- Access the AWS Console.
- Create S3 bucket and add required contents in that such as index.html, error.html or images.
- Provide permissions and attach the policy.
- To create a distribution.
	- Enter the origin domain - In this case the S3 bucket name (eg - jrastos-cloudfront.s3.us-east-2.amazonaws.com)
	- Select to use the OAI "Yes use OAI (bucket can restrict access to only CloudFront)" 
	- If you want to use HTTPS protocol then you need to provide SSL Certificate or you can use CloudFront SSL certificate which is paid.
    - So if you don't want to use HTTPS select HTTP protocol.
	- Hop on to Settings bar and mention the static website page "Default root object" eg - index.html
    - If you want access default web page directly then you need to mention "Default root object" such as index.html
      ![Default_root](/uploads/dd873f5b86f4b83bd1a61f583c28baaa/Default_root.PNG)
    - If you don't want to provide default root then you need provide file name after your domain name 
      for example `(tikufinalcf.s3.us-east-2.amazonaws.com/index.html)`
	- Click on create distribution.
	- It will take sometime to be in Deploy state.

## Conclusion :-

When you use domain name to access then you will get below output.

![Output2](/uploads/d3ba033e78e4a9c4891fe8be3c4f8920/Output2.PNG)
