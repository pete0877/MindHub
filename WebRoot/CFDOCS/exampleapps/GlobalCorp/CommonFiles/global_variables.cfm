<!--- ColdFusion(r) Express Global Corp. Example Application --->



<!--- The name of the datasource used througout the site --->

<CFSET application.ds="GlobalCorpDB">



<!--- Colors used throughout the site --->

<CFSET application.darkorange="##cc6600">

<CFSET application.lightorange="##ff9900">

<CFSET application.lightgray="##CCCCCC">

<CFSET application.darkgray="##999999">

<CFSET application.black="##000000">

<CFSET application.white="##FFFFFF">





<!--- Body tag variables these are used in the common file _header.cfm --->

<CFSET application.bgcolor=application.darkorange>

<CFSET application.font_color=application.black>

<CFSET application.link_color=application.darkorange>

<CFSET application.followed_link_color=application.darkorange>

<CFSET application.anchor_link_color=application.darkorange>



<!--- Display variables --->

<CFSET application.font="Arial">

<CFSET application.table_headcolor=application.darkgray> 

<CFSET application.table_cellcolor=application.lightgray>

 

<!--- Path variables --->



<CFSET application.root_path="../GlobalCorp/">

<!--- <CFSET application.root_path="/cfdocs/exampleapps/GlobalCorp/"> --->

<CFSET application.image_path="#application.root_path#Images/"> 

<CFSET application.employee_image_path="#application.image_path#people/">


<!--- Variables used in the meeting room scheduler --->

<CFSET application.months="January,February,March,April,May,June,July,August,September,October,November,December">

<CFSET application.displayrangelist="Anytime,This Week,Next Week,Next Two Weeks,This Month,Next Month,Next Two Months">



<!--- Variable to Designate that the Global Variables have been created --->

<CFSET application.initialized=1>



