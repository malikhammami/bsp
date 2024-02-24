def notifySuccess() {
    def imageUrl = 'https://www.weodeo.com/wp-content/uploads/2023/02/DevOps-scaled.webp' // Replace with the actual URL of your image
    def imageWidth = '800px' // Set the desired width in pixels
    def imageHeight = 'auto' // Set 'auto' to maintain the aspect ratio

    // Read the entire console log file
    def consoleLog = readFile("${JENKINS_HOME}/jobs/${JOB_NAME}/builds/${BUILD_NUMBER}/log")
    def logFile = "${WORKSPACE}/console.log"
    writeFile file: logFile, text: consoleLog

    emailext(
        body: """
            <html>
                <body>
                    <p>YEEEEY, The Jenkins job was successful.</p>
                    <p>You can view the build at: <a href="${BUILD_URL}">${BUILD_URL}</a></p>
                    <p><img src="${imageUrl}" alt="Your Image" width="${imageWidth}" height="${imageHeight}"></p>
                    <p>Console Log is attached.</p>
                </body>
            </html>
        """,
        subject: "Jenkins Job - Success",
        to: 'malik.hammami1@gmail.com',
        attachLog: true,  // Attach the log file
        attachmentsPattern: logFile,  // Specify the file to attach
        mimeType: 'text/html'
    )
}

pipeline {
    agent any
    parameters {
        string(name: 'BRANCH_NAME', defaultValue: "${scm.branches[0].name}", description: 'Git branch name')
        string(name: 'CHANGE_ID', defaultValue: '', description: 'Git change ID for merge requests')
        string(name: 'CHANGE_TARGET', defaultValue: '', description: 'Git change ID for the target merge requests')
    }
    environment {
        DOCKERHUB_CREDENTIALS = credentials('docker_cred')
    }
    stages {
        stage('Github Auth') {
            steps {
                script {
                    branchName = params.BRANCH_NAME
                    targetBranch = branchName

                    git branch: branchName,
                    url: 'https://github.com/malikhammami/bsp.git',
                    credentialsId: 'git_cred'
                }
                echo "Current branch name: ${branchName}"
                echo "Current branch name: ${targetBranch}"
            }
        }
        stage('Restore and Build') {
    steps {
        dir('MicroServicePayment') {
            sh 'dotnet restore'
            sh 'dotnet build'
        }
    }
}
        stage('Test') {
            steps {
                sh 'dotnet test'
            }
        }

      stage('DockerHub login and image build') {
            steps {
                    script {
                             sh 'echo $DOCKERHUB_CREDENTIALS_PSW | docker login -u $DOCKERHUB_CREDENTIALS_USR --password-stdin'
                           }
                sh 'docker build -t malikhammami99/testdotnet .'


            }
        }

        stage('Pushing Image to DockerHub') {
            steps {
                sh 'docker push malikhammami99/testdotnet'

            }
        }
       
        stage('Email Notification') {
            steps {
                script {
                    currentBuild.resultIsBetterOrEqualTo('SUCCESS') ? notifySuccess() : notifyFailure()
                }
            }
        }
    }

    post {
        always {
            sh 'docker logout'
        }
    }
}
