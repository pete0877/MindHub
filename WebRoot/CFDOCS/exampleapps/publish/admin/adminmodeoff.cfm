<CFCOOKIE NAME="PubAdminMode" VALUE="1" EXPIRES="NOW">
<SCRIPT LANGUAGE="JavaScript">
location.href = '<CFOUTPUT>#CGI.HTTP_REFERER#</CFOUTPUT>'
</SCRIPT>