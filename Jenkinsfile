pipeline {
  agent { any }  // Allows running on any agent

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