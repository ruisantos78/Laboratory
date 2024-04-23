pipeline {
    agent {
        any
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
