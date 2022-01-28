# CloudEngineering-Training

## Problem Statement:-
1. Run the NGINX on ASG using userdata file.
2. Create your custom nginx.conf file
3. Pick the contents from the S3 bucket and host the static website.
4. Push the logs to CloudWatch.

## Sample Infrastructure Diagram:-

![infra](/uploads/284ea9f669e9d49b8b4f0eccb21560e6/infra.PNG)

## Services Used
### EC2
This service is use for showing the list of the instances. By using this we can easily see the all information about the instances.

![EC2_instance](/uploads/e2e3f33213c2f185c5151a894fc05556/EC2_instance.png)


### Load Balancer
This service was used for balance the load between the running instances.

![LoadBalancer](/uploads/7244163a29377cc6bbe75282879515f3/LoadBalancer.png)


### Auto Scaling Groups
Using this service we can automatically scale up and scale down the instances as per the requirement.

![ASG](/uploads/87588935135ea22efd69273f3834285d/ASG.png)


### Launch Template
To create ***ASG*** we need this template because in this template we already mentioned our requirements.

![LaunchTemplate](/uploads/7eb02a1ea65bfaf6de782e7a0f28925a/LaunchTemplate.PNG)


### CloudWatch Logs
To check the logs if any issue happens this service is important, by using this we can check the status of the sites.

![Access-ErrorLogs](/uploads/ba6dfe275473227c09dbbe3733a4bb80/Access-ErrorLogs.PNG)


### Bucket Policy
This policy is important because of this we can access our data which stored on Amazon S3.
Once you create this policy then you must attach it to your ***IAM Roles***.

```
{
    "Version": "2012-10-17",
    "Statement": [
        {
            "Sid": "PublicReadGetObject",
            "Effect": "Allow",
            "Principal": "*",
            "Action": "s3:GetObject",
            "Resource": "arn:aws:s3:::tikubuckt/*"
        }
    ]
}

```


### S3 bucket
This is nothing but our storage location where we store the data in terms of files, images, codes etc.

![S3_Bucket](/uploads/d825bb3f020d179f4ebc7bea6a0284f1/S3_Bucket.PNG)


### IAM Role Policies
These roles are important in this we added different policies which is related to your instances.

![IAMRolePolicies](/uploads/6d01c524e8209da59b33a1fc2b188475/IAMRolePolicies.PNG)


## User-Data Created as follows
### By using this the commands NGINX will run on every boot.

```
Content-Type: multipart/mixed; boundary="===============5189065377222898407=="
MIME-Version: 1.0

--===============5189065377222898407==
Content-Type: text/cloud-config; charset="us-ascii"
MIME-Version: 1.0
Content-Transfer-Encoding: 7bit
Content-Disposition: attachment; filename="cloud-config.txt"

#cloud-config
repo_upgrade: none
repo_releasever: 2.0
cloud_final_modules:
 - [scripts-user, always]

--===============5189065377222898407==
Content-Type: text/x-shellscript; charset="us-ascii"
MIME-Version: 1.0

Content-Transfer-Encoding: 7bit
Content-Disposition: attachment; filename="user-data.txt"

#!/bin/bash
sudo su
sudo apt-get update	
sudo apt install nginx -y	
sudo systemctl start nginx
sudo systemctl enable nginx
sudo rm nginx.conf
sudo apt install awscli -y
sudo aws s3 cp s3://<your_bucket_name>/index.html
sudo aws s3 cp s3://<your_bucket_name>/nginx.conf /etc/nginx
sudo systemctl restart nginx
sudo service awslogs restart
```



## NGINX Configuration file having containts are as follows:-

```
events {
  worker_connections  768;
}

http {
  default_type       html;

  sendfile           on;
  keepalive_timeout  65;

  proxy_cache_path   /tmp/ levels=1:2 keys_zone=s3_cache:10m max_size=500m
                   inactive=60m use_temp_path=off;


server {
  listen 80;
  listen 443 ssl;
  server_name  18.221.112.221;
  set $bucket "tikubuckt.s3.us-east-2.amazonaws.com";
  root         https://tikubuckt.s3.us-east-2.amazonaws.com/index.html
  sendfile on;

location / {
    resolver 8.8.8.8;
    proxy_http_version     1.1;
    proxy_redirect off;
    proxy_set_header       Connection "";
    proxy_set_header       Authorization '';
    proxy_set_header       Host $bucket;
    proxy_set_header       X-Real-IP $remote_addr;
    proxy_set_header       X-Forwarded-For $proxy_add_x_forwarded_for;
    proxy_hide_header      x-amz-id-2;
    proxy_hide_header      x-amz-request-id;
    proxy_hide_header      x-amz-meta-server-side-encryption;
    proxy_hide_header      x-amz-server-side-encryption;
    proxy_hide_header      Set-Cookie;
    proxy_ignore_headers   Set-Cookie;
    proxy_intercept_errors on;
    add_header             Cache-Control max-age=31536000;
    proxy_pass             https://$bucket; # without trailing slash
    root                   https://tikubuckt.s3.us-east-2.amazonaws.com/index.html
  }
}

}
```

## CloudWatchAgent.json file containts are as follows:-

```
{
"agent": {
   "metrics_collection_interval": 60,
   "region": "us-east-2",
   "logfile": "/opt/aws/amazon-cloudwatch-agent/logs/amazon-cloudwatch-agent.log",
   "debug": false,
   "run_as_user": "root"
  },

    "metrics": {
      "namespace": "CWAgent",
     "metrics_collected": {
        "disk": {
            "resources": [
              "/",
              "/tmp"
            ],
            "measurement": [
              {"name": "free", "rename": "DISK_FREE", "unit": "Gigabytes"},
              "total",
              "used"
            ],
             "ignore_file_system_types": [
              "sysfs", "devtmpfs"
            ],
            "metrics_collection_interval": 60
          }
        }
    },
    "logs": {
            "logs_collected": {
              "files": {
                "collect_list": [
                    {
                        "file_path": "/var/log/nginx/access.log",
                        "log_group_name": "/ec2/nginx/logs",
                        "log_stream_name": "access.log",
                        "initial_position": "end_of_file"
    
                      },
                      {
                        "file_path": "/var/log/nginx/error.log",
                        "log_group_name": "/ec2/nginx/logs",
                        "log_stream_name": "error.log",
                        "initial_position": "end_of_file"
                      }
                    ]
                  }
                },
                "log_stream_name": "my_log_stream_name",
                "force_flush_interval" : 15
              }
}                              

```
## To generate cloudwatch logs we need to create policy for that and then we can attach that policy to existing IAM Role or you can create new role which have below containts:-

```
{
  "Version": "2012-10-17",
  "Statement": [
    {
      "Effect": "Allow",
      "Action": [
        "logs:CreateLogGroup",
        "logs:CreateLogStream",
        "logs:PutLogEvents",
        "logs:DescribeLogStreams"
    ],
      "Resource": [
        "arn:aws:logs:*:*:*"
    ]
  }
 ]
}
```


## To generate NGINX logs we need add entries in this file [/var/awslogs/etc/awslogs.conf]
So by using this log file aws will create log group "/ec2/nginx/logs" and inside that required logs folder will be created i.e. access.log as well as error.log

```
[/var/log/nginx/access.log]
datetime_format = %d/%b/%Y:%H:%M:%S %z
file = /var/log/nginx/access.log
buffer_duration = 5000
log_stream_name = access.log
initial_position = end_of_file
log_group_name = /ec2/nginx/logs

[/var/log/nginx/error.log]
datetime_format = %Y/%m/%d %H:%M:%S
file = /var/log/nginx/error.log
buffer_duration = 5000
log_stream_name = error.log
initial_position = end_of_file
log_group_name = /ec2/nginx/logs
```


### Result
Below is the expected output


![output](/uploads/3eb2f4ad62afa6bddd5c698b2129297f/output.PNG)




