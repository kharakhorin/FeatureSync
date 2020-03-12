# FeatureSync
[![Build Status](https://kharakhorin.visualstudio.com/FeatureSync/_apis/build/status/kharakhorin.FeatureSync?branchName=master)](https://kharakhorin.visualstudio.com/FeatureSync/_build/latest?definitionId=1&branchName=master)

Synchronization tool for integrating SpecFlow scenarios with Azure DevOps (TFS)

## How to use

### 1. Add namespace and language definition to feature file:
```gherkin
#language:en
@Namespace:Application.Autotests
Feature: Log to application
```
*namespace must be equal to test .dll name

### 2. Create empty Test Cases in TFS and add tags with id to scenario:
>![Empty case](https://github.com/kharakhorin/FeatureSync/blob/master/Docs/Img/empty_case.JPG)
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

![Buld definition](https://github.com/kharakhorin/FeatureSync/blob/master/Docs/Img/build_def.JPG)

### Profit
>![Sync case](https://github.com/kharakhorin/FeatureSync/blob/master/Docs/Img/sync_case.JPG)
***
>![Automation](https://github.com/kharakhorin/FeatureSync/blob/master/Docs/Img/automation_case.JPG)
