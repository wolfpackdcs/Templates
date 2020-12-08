# Templates
This repository contains templates to use in the Wolfpack ORP ecosystem. To be able to use these templates you will need access to the Wolfpack Nuget repository. Your account manager can help you in getting credentials to get access to this repository.

To install the templates follow the steps below:

1. Open command prompt, in Administrator mode.
2. Install the templates: dotnet new -i [RootTemplateFolder]
The RootTemplateFolder is the folder containing the .template.config folder. This folder contains the configuration to make the templating work.
3. To create an application from the template the following command needs to be executed: dotnet new [template-short-name] -o [directory] -n [name]

See also: https://docs.microsoft.com/nl-nl/dotnet/core/tools/custom-templates

