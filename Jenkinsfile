pipeline {
    agent {
        docker {
            image 'mcr.microsoft.com/dotnet/sdk:7.0'
            args '-v /var/run/docker.sock:/var/run/docker.sock'
        }
    }
    
    stages {
        stage('Build') {
            steps {
                script {
                    sh 'dotnet restore'
                    sh 'dotnet build'
                }
            }
        }

         stage('Test') {
            steps {
                script {
                    sh 'dotnet test'
                }
            }
        }
    }
}
