pipeline {
    agent {
        dockerfile {
            filename 'Dockerfile'
            dir '.'
        }
    }
    
    stages {
        stage('Build') {
            steps {
                script {
                    // Restore NuGet packages
                    sh 'dotnet restore'

                    // Build the .NET app using MSBuild
                    sh 'dotnet build'
                }
            }
        }
    }
}
