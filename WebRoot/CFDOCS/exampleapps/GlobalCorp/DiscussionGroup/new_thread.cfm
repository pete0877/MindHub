<!--- ColdFusion(r) Express Global Corp. Example Application --->


<!--- Set a default form value. --->
<CFPARAM NAME="Form.threadname" DEFAULT="">

<!--- 	We only create a thread if the user typed something in. notice the use of len(trim()). this does the following: first, it calls trim on the value to remove any extra spaces at the beginning or end of the value. we then use 	the len function. if the user didn't type anything in, or if s/he typed in just spaces, the value of len after the trim will be 0. <cfif 0> equals false and the code within won't be called. we could have done <cfif trim(form.threadname) eq ""> 	but the len function runs a bit faster. --->
<CFIF Len(Trim(Form.threadname))>
	<!--- Insert thread into db --->
	<CFSET TheTime=CreateODBCDateTime(Now())>
	<CFSET NEW_ID = CreateUUID()>
	<CFQUERY NAME="CreateThread" DATASOURCE="#application.ds#">
		INSERT INTO tblThrd(strThrdID,strThrd,dtCreated)
		VALUES('#NEW_ID#','#HTMLEditFormat(Form.threadname)#',#TheTime#)
	</CFQUERY>
	<!--- We force them to go post a message on the thread they just made. --->
	<CFLOCATION URL="post_message.cfm?id=#NEW_ID#" ADDTOKEN="No">
</CFIF>

<!--- Include the Global Header --->
<CFSET header.page_name="Create New Thread">
<CFINCLUDE TEMPLATE="../CommonFiles/_header.cfm">


<!--- Discussion Group Menu --->
<FONT SIZE="2">
{
<A HREF="index.cfm">Thread List</A>
 | 
<A HREF="profile.cfm">Customize Profle</A>
 | 
<A HREF="search.cfm">Search Messages</A>
}
</FONT>


<FORM ACTION="new_thread.cfm" METHOD="POST">
<INPUT TYPE="hidden" NAME="gogo" VALUE="1">
Name of thread: 
<INPUT TYPE="text" NAME="threadname" SIZE="30" MAXLENGTH="60"> 
<INPUT TYPE="submit" VALUE="Create Thread and Compose Message">
</FORM>


<!--- Error display --->
<CFIF IsDefined("Form.gogo") AND NOT Len(Trim(Form.threadname))>
<B>You must enter the name of the thread to create it.</B>
<P>
</CFIF>




<!--- Include the Global Footer --->
<CFINCLUDE TEMPLATE="../CommonFiles/_footer.cfm">