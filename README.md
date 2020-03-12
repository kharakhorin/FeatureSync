# FeatureSync
Synchronization tool for integrating BDD scenarios with Azure DevOps (TFS)

## How to use

### 1. Add namespace and language definition to feature file:
```gherkin
#language:en
@Namespace:Application.Autotests
Feature: Log to application
```
*namespace must be equal to your .dll name

### 2. Create empty Test Cases in TFS and add tags with id to scenario:
![Selenoid Animation](Docs/Img/empty_case.jpg)
```gherkin
@2122409 @posistive
Scenario: Successful authorization
	Given I on authorization page
	And I enter:
		| Login | Password |
		| user  | pass     |
	When I press Login button
	Then Browser redirect on Home page
```

### 3. Run FeatureSync:
```
FeatureSync.exe -f C:\FolderWithFeatures -s https://tfs.server.com/collection -t 6ppjfdysk-your-tfs-token-2d7sjwfbj7rzba
```

I run FeatureSync after buld of test solution
![Selenoid Animation](Docs/Img/build_def.jpg)

### ?. Profit
![Selenoid Animation](Docs/Img/sync_case.jpg)
![Selenoid Animation](Docs/Img/automation_case.jpg)
