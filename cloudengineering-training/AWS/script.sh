#!/bin/bash
aws ec2 run-instances --image-id ami-0d718c3d715cec4a7 --count 1 --instance-type t2.micro --key-name tejas-key-pair --security-groups-ids sg-0d6e916074f72883b --region us-east-2
