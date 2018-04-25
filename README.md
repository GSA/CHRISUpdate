# CHRISUpdate

## Description: 
Imports Employee update and separation files from HRLinks and updates database on matching records.


## Nuget packages and other dll's utilized
* CsvHelper (https://www.nuget.org/packages/CsvHelper/2.16.3/)
* log4net (https://www.nuget.org/packages/log4net/2.0.8/)
* MySQL.Data (https://www.nuget.org/packages/MySql.Data/6.9.9/)
* Google.libphonenumber (https://www.nuget.org/packages/libphonenumber-csharp/)
* AutoMapper (https://www.nuget.org/packages/AutoMapper/7.0.0-alpha-0001)
* FluentValidation (https://www.nuget.org/packages/FluentValidation/7.6.0-preview1)



## Initial Setup
### Default config setup

The repository may contains an app config that points to external config files. These external config files not controlled by version control will need to be created and configured prior to running the application. The files required and the default configuration can be found below. For those on the development team, additional details can be found in the documentation on the google drive in the GIT team drive.


 * **Things to do before your first commit**
   * Make a new branch for development. All pre-existing branches are protected and cannot be pushed to directly.
   * You can publish a new branch and do pull requests to have your changes incorporated into the project.
   * Once you have created a new branch you will need to create the config files. (see below for more info on this)
   * Default version of these files are provided in the repo with the .example extension
   * Copy these files into the project **bin\Debug folder** and change the extension to .config using the previous filename
   * Or create new files that contain the code as seen below and place them in the **bin\Debug folder**
   * Do not push your config files to the repository. Pull requests that include these files will be rejected.
 
 * **Current config files that will need to be added.**
   * ConnectionStrings.config
   * AppSettings.config
 
* **Default settings for these files will follow this line**
 
   * **ConnectionStrings.config file should contain the following lines.** 
    ~~~ xml
    <connectionStrings>
      <add name="GCIMS" connectionString="Server=[IP Address]; Port=[port]; user id=[username]; password=[password];persist security info=True;database=hspd; pooling=true;" providerName="MySql.Data.MySqlClient" />
      <add name="HR" connectionString="Server=[IP Address]; Port=[port]; user id=[username]; password=[password];persist security info=True;database=employees; pooling=true;" providerName="MySql.Data.MySqlClient" />
    </connectionStrings>
    ~~~

   * **AppSettings.config should contain the following lines.**
  ~~~ xml
  <appSettings>
    <add key="KEY" value="REPLACEME" />
    <add key="CHRISFILENAME" value="C:\Temp\chris\smallfile.dat"/> <!-- CHRISData.dat || smallfile.dat -->
    <add key="SEPARATIONFILENAME" value="CHRISGCIMSEXEMPOUT.dat"/> <!-- SeparationData.dat -->
    <add key="ORGFILENAME" value="OrgData.dat"/>
    <add key="LOADPII" value="false"/>
  </appSettings>
  ~~~
  
  ***
  
## Usage
Executable file that uses files from HRLinks recieved by sftp and inserts the data into the database for matching records.

## Contributing
Fork this repository, make changes in your fork, and then submit a pull-request, remembering not to upload any system specific configuration files, PII, or sensitive data of any type. 

## Credits
GSA
