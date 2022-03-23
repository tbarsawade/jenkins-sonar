# How to setup a Jenkins Build Server on AWS Linux EC2 instance.
- Create a new EC2 instance
- Connect to EC2 instance using SSH
- Setup Jenkins on EC2

## EC2 Server Commands to install Jenkins:-
- #### Update all software packages on ec2 server:-
  
  sudo yum update

- #### Add Jenkins repo:-
  
  sudo wget -O /etc/yum.repos.d/jenkins.repo http://pkg.jenkins-ci.org/redhat/jenkins.repo

- #### Import a key file from Jenkins-CI to enable installation from the package:-
  
  sudo rpm --import https://pkg.jenkins.io/redhat/jenkins.io.key

- #### Install Jenkins:-
  sudo yum install jenkins -y

- #### Update Java:-
  sudo yum install java-1.8.0

- #### Switch to Java:-
  sudo alternatives --config java

- #### Start Jenkins as a service:-
  sudo service jenkins start

- #### Get Admin Password:-
  sudo cat /var/lib/jenkins/secrets/initialAdminPassword

# Result:-

**Jenkins URL:- http://{YOUR INSTANCE PUBLIC IP ADDRESS}:8080/**
