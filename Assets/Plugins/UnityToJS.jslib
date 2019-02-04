mergeInto(LibraryManager.library, {

    SubmitDescription: function() {
        var returnStr = window.prompt("Enter a description:", "A red box");
		
		if(returnStr == null){
			console.log("The user has cancelled the input event.");
			return null;
		}
		
        var bufferSize = lengthBytesUTF8(returnStr) + 1;
        var buffer = _malloc(bufferSize);
        stringToUTF8(returnStr, buffer, bufferSize);
        return buffer;
    },
	
	SubmitDescriptionB: function() {
		while(true){
			var desc = window.prompt("Enter a description: ", "");		
			
			if(desc == null)
				continue;
		    break;							
		}
		
		var bufferSize = lengthBytesUTF8(desc) + 1;
        var buffer = _malloc(bufferSize);
        stringToUTF8(desc, buffer, bufferSize);
        return buffer;			
	},

    PassArgs: function() {
        var params = window.location.search.substring(1);		
        var size = lengthBytesUTF8(params) + 1;
        var buffer = _malloc(size);
        stringToUTF8(params, buffer, size);
		console.log(buffer);
        return buffer;
    },
	
	OpenUrl : function(url) {
		window.open(Pointer_stringify(url));
		console.log("Opening URL.");
		console.log(Pointer_stringify(url));
	},
	
	Close : function() {
		console.log("Need to modify UnityLoader to load the game within a new script created tab in order to close the window for quit.");
		window.open('', '_self', '').close();
	},
	
});