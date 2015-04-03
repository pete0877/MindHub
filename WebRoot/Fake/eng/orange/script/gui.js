// ProcessEvent() - handles processing of an event.
function HandleEvent(senderID, eventName, eventArguments)
{
	document.gui.vguic.value = 'handleevent';
	document.gui.vguia.value = senderID + ',' + eventName + ',' + eventArguments;
	document.gui.submit();
}

// ReturnToToday() - returns the user to Today-screen.
function ReturnToToday()
{
	window.location.href = "/webapi/process.aspx?vf=displaytoday";
}

