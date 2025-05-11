SCENARIO:
- One page with acces just for the authenticated users ("/protected")
- Login button (with login in zitadel)
- Logout button 
- Display user info from the user claims (in angular)
- "Call Protected API" button with API action (on click the api is called). That api will check the policy in backend and return the role and tenant if there is access for that role/tenant combination. Check the policy in backend in "Casbin/policy.csv".
- "Visible just for Role2" button visible in fe just for the users from tenant1 with Role2. This access is checked in front-end (using casbin js sdk). The policy from fe is in "assets\casbin\policy.csv". All the policies should be moved in the database in the same place (be policy and fe policy), should be only one source for both of them. Actual configuration is just in scope of POC.
- Silent refresh token implemented


ZITADEL:

- install zitadel using the latest Docker image ghcr.io/zitadel/zitadel:latest
- create a new project in zitadel (projects -> create new project)
- in project "General" tab - check "Assert Roles on Authentication"
- in project "Roles" tab add 2 new options "Role1" & "Role2" (will be used later for testing)
- in project "Grants" tab grant the new 2 added roles
- in project "AUthorizations" tab add these 2 new roles to the "ZITADEL Admin" user. (will be used later for testing). This action can be done also from the user administration (Users from the main menu)
- go Back in General Tab and click on "New" button and add a new applicaiton
- Select "WEB" as app type
- Select PKCE as authentication method
- In redirect urls enter the url from your .net api instance (ex. http://localhost:4200). You can add it later if the .net api is not running
- After the application was added then go to it (project-> Click on app in APPLICATIONS section)
- In "Configuration" tab -> Be sure that "response type" is "Code", Check "Refresh token", Grant types - check "Authorization Code, Refresh TOken"
- In "Token Settings" select "JWT" in "AUTH Token Type", Check all the checkboxes from this section
- In "Redirect Settings" complete with the .net api instance (ex. http://localhost:4200)
- Go to the main menu "Users" -> select "Zitadel Admin" user -> On authorizations be sure that is authorized to the current project and be sure that has the 2 roles added (Role1 & Role 2), Go to metadata and add a new metadata (key-> "tenant", value -> "tenant1") - all these will be used later in testing
- Go to projects -> select current project -> select created application and go in "Configuration" tab. Copy "Client ID" value and save it. This will be used later in the .net api.


ANGULAR:
- go in ap -> services -> hello.service.ts and change the url with the url of your .net api instance (line 8)
- go in src -> app -> app.config.ts and:
	- change the issuer (should be the address of the zitadel instance ex. http://localhost:8080) line 11
	- change the ClientID (should be the value specified in the last point of ZITADEL steps)
- in assets -> casbin -> policy.csv you can see the policy definition (ex. p, Role2, button:role2, view - Role2 can see the button:role2)

.NET
- go in Program.cs and change the authority (line 35). Should be the url of the zitadel instance ex. http://localhost:8080
- in Casbin -> policy.csv you can see the policy definition 



