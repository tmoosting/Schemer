# Schemer
<sub>Graduation Research Project for Masters program: [Media Technology](https://www.universiteitleiden.nl/en/education/study-programmes/master/media-technology) in Leiden.
</sub> 


A data framework with Unity application that translates political settings through an SQLite database into political hierarchies which can  simulate political power dynamics through a set of actions.

Original research question: 

*Can the essentials of political power be represented in a flexible digital platform that allows playful exploration of the concept?*

### Research Paper
eta dec 2021





### Running in Unity
- Required Version: 2020.x or 2021.x
- Download files and extract, or clone repository
- Open Assets folder in Unity Hub to load project
- In Hierarchy, navigate to DataLinker under DataController to change default database name in Inspector
- In Hierarchy, click the UIController to see visual customization options in Inspector


### Running Executable
- Download the Windows or Mac version from Builds folder
- Extract archive and run executable file
- Read the instructions on opening screen for loading a database
- View objects and take actions 
- Hover the '?' button in the application for more information

### Using a Custom Database
- Download an SQLite database editor tool such as [DB Browser for SQLite](https://sqlitebrowser.org/) or [DBeaver](https://dbeaver.io/)
- Open Template.db from the Assets folder and save as new database
- Create your Characters, Materials and Institutions into each table
- ID fields cannot be null
- Fields that start with 'Own' or 'Coop' hold one or more IDs, separated by commas, no spaces 
- See section 5 of the research paper for more detailed information on each field
- Load your database through the Unity project or executable version

### Framework Data Types

#### Character (CHA)
- Represents any individual actor

#### Material (MAT)
- Represents any physical object that is not a Character

#### Institution (INS)
- Represents any type of collective association or political organisation

#### Relation (REL)
- Specifies a cooperation or ownership connection between two objects

### Actions

#### Kill Character / Destroy Material / Disband Institution / Remove Relation
- Destroy Primary Selected Object
- Destroys the object in-code, along with any Relations that involved the object
- Inheritance logic creates new Relations where applicable

#### Gift Material
- A Material object of the chosen Subtype and amount is created for Primary Selected Object
- Available for CHA and INS objects

#### Claim Ownership
- Primary Selected Object (CHA or INS) takes ownership of Secondary Selected Object (CHA or MAT or INS)

#### Create Cooperation
- Create a Cooperation-Relation between Primary Selected Object (CHA or INS) and Secondary Selected Object (CHA or INS)

#### Break Cooperation
- If Cooperation Relation exists between Primary Selected Object and Secondary Selected Object: destroy it

