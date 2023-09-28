# AWS dotnetcore

## Create DockerFile, make sure "docker build" and "docker run" works
## Create buildspec.yml
## Create new repository in ECR

link https://us-east-1.console.aws.amazon.com/ecr/create-repository

Options: 
- Private
- Repository name is projects name (testpdf)
- everything else as default

## Create Pipeline using CodePipeline

### Step #1 - Choose pipeline settings

options:
- Pipeline name = project name
- if first time, create `new service role`
  - use defaults for role name
  - check `Allow AWS CodePipeline to create a service role so it can be used with this new pipeline`
- click `Next`

### Step #2 - Add source Stage

- Source provider: `Github v1`
- Select project repository
- Select project main branch
- in Change detection options, use `AWS CodePipeline`

### Step #3 - Add build stage

- Build Provider: `AWS CodeBuild`
- Region: `US East (N. Virginia)`
- Project name: `Create project`

#### Create Build project | CodeBuild

- Project name: testpdf

##### Environment

- Environment Image: `Managed image`
- Operating system: `Ubuntu`
- Runtime `Standard`
- Image: latest available ( atm latest is `aws/codebuild/standard:7.0`)
- Image version: `Always use the latest image for this runtime version`
- Environment type: `Linux EC2`
- Privileged: `check`
- Service role: if first time, choose `New service role` (codebuild-testpdf-service-role)
- Additional configuration: Here we leave everything as default and we add env vars that we are using in our buildspec project 

env vars example:

```
AWS_DEFAULT_REGION=us-east-1
AWS_ACCOUNT_ID=086414240535
IMAGE_REPO_NAME=testpdf
IMAGE_TAG=latest
CONTAINER_NAME=testpdf
```

- build specifications: `Use a buildspec file`
- cloudwatch logs: `check`
- `Continue to CodePipeline`

Before continue to Step #4, we go to IAM in another tab and look for the recently created role for codebuild, it should be named like `codebuild-<project>-service-role`.

- Open it
- click in add permissions
- search for `AmazonEC2ContainerRegistryFullAccess`
- click `Add permissions`
- now we can go back to CodePipeline and we proceed with `Next`

### Step #4 - Add deploy stage

- `Skip deploy stage`

### Step #5 - Review

- `Create pipeline`

## Try build
