pipeline {
    agent any
    parameters {
	string(
		name: "BRANCH_NAME",
		description: "Branch name that you want to build",
		defaultValue: "develop"
		)
    choice(
		name: "WORKFLOW",
		description: "Select the environment you want to deploy your code on",
		choices : ["Build","Deploy"]
		)
    string(
        name: 'CommitID', defaultValue: 'latest', 
        description: 'commit id refernce used for building package'
        )
	}
    stages {
        stage('build') {
            steps {
                echo "==========Building========="
                bat '''
                msbuild /property:Configuration=Release Myend_sampleApp\\WebApplication1\\WebApplication1.sln
                '''
            }
        }
        stage('Test') {
            steps {
                echo "==========Testing========="
                echo "No test cases to execute"
            }
        }
        stage('Zip artifact') {
            when{
                expression{params.BRANCH_NAME == "develop"}
            }
            steps {
                echo "==========Zipping========="
                bat '''
                tar -czvf bin.zip Myend_sampleApp\\WebApplication1\\bin
                '''
            }
        }
        stage('Upload')
        {
            when{
                expression{params.BRANCH_NAME == "develop"}
            }
            steps
            {
                ftpPublisher alwaysPublishFromMaster: true,
                continueOnError: false,
                failOnError: false,
                masterNodeName: '',
                paramPublish: null,
                publishers: [[configName: 'ArtefactFtp', transfers: [[asciiMode: false, cleanRemote: true, excludes: '', flatten: false, makeEmptyDirs: false, noDefaultExcludes: false, patternSeparator: '[, ]+', remoteDirectory: '', remoteDirectorySDF: false, removePrefix: '', sourceFiles: 'bin.zip']], usePromotionTimestamp: false, useWorkspaceInPromotion: false, verbose: false]]

                
            }
        }
        stage('Download artefact')
        {
            when{
                expression{params.BRANCH_NAME == "develop"}
            }
            steps
            {
                withCredentials([string(credentialsId: 'ftp', variable: 'ftp_pass')]) {
                bat '''
                curl -O -u testuser:$ftp_pass ftp://3.133.79.138/bin.zip
                '''
                }
            }
        }
        stage("Create git tag"){
        when {
            expression {params.WORKFLOW == "Deploy"}
        }
        steps{
        echo "Creating git tags"
        withCredentials([sshUserPrivateKey(credentialsId: 'gitlab-ssh-key', keyFileVariable: 'id_rsa')]){
            script{
            bat '''
            git rev-parse --short %CommitID%>tag.txt
            set /P tag=<tag.txt
            git tag %tag% %tag% -m \"Tag created through jenkins\"
            type id_rsa>gitlab-ssh-private-key-gokloud
            git push --force origin %tag%
            '''
            }
        }
        }
        } 

    }
}
