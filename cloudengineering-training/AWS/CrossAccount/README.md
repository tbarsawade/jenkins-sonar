# Setup Cross Account Access:-

1. You need TWO AWS accounts for this exercise. Say Account A and Account B.
   NOTE:- You must have account ids for both accounts.
2. Create IAM Role in A with name "Role_for_B" for giving access to users in Account B.
![Role_for_account_B](/uploads/7092d721c87b55dcd5fe14bb20276101/Role_for_account_B.PNG)

3. Attach EC2ReadPermissions to this role in Account A
4. In Account B, create an IAM user or use existing IAM user.
![New_User_Created_In_Account_B](/uploads/04c19a742e09aa73439e4cc7b8bffc13/New_User_Created_In_Account_B.PNG)

5. Attach AssumeRole policy.
```
{
  "Version": "2012-10-17",
  "Statement": {
    "Effect": "Allow",
    "Action": "sts:AssumeRole",
    "Resource": "arn:aws:iam::349341122682:role/Role_for_B" #[This is nothing but role ARN which we copied from Account A]
  }
}
```
**What this policy does is "It allows the users in account B to assume the role in account A", When you do a switch role you leave apart your existing permissions for existing account and You just only operate a new account with the new permissions which are attached.**


6. Login to console for Account B and Switch Role to Role_A_for_B

7. Validate that you can see EC2 instances in Account A

8. Account "B" should access account "A" data.


# Result:-

As per role we have only provided the EC2 and S3 read access to the account
that means the account only have read access to EC2 as well as S3.

![EC2_Read_Access](/uploads/8d7ff3a06c0d89f344c891d793fe9620/EC2_Read_Access.PNG)

Apart from this all the other services access has been denied to this user.

![IAM_access_denied](/uploads/789ae45215771d30a969d83bb46b0e0d/IAM_access_denied.PNG)
