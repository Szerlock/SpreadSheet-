/*
 * H. James de St. Germain
 * Fall 2024
 *
 * Sample Simple Javascript that could be used in Blazor.
 * Note: in as many cases as possible, DO NOT USE JAVASCRIPT! Use
 * blazor and C# instead. In some cases, like the browser beforeunload
 * event, you have no choice but to use JavaScript
 *
 * IMPORTANT:
 *   0) This file must have the property "Copy to Output Directory" set (i.e., to Newer)
 *   1) The first method called by Blazor must be: SetDotNetInterfaceObject
 *   2) All functions that Blazor will call must be **Exported**
 */

// The DotNet interface object
var dotNetInterface;

/**
 *  Function to save the dotNetInterface object for future use.
 *  This is called once at the start of the program.
 * 
 * @param {any} DNI - the dot net object for communicating back to C#
 */
export function SetDotNetInterfaceObject(DNI)
{
    dotNetInterface = DNI;
}

/**
 * This is an example of how to connect a JavaScript function
 * to C# and vice versa.  
 * 
 * (1) This method is called by Blazor in the First Rendering of the Page
 * 
 * (2) This function then calls the Blazor (C#) method: TestBlazorInterop.
 * 
 * @param {string} message - the message received from C#
 */
export function TestJavaScriptInterop( message )
{
    console.log( `JSTestJavaScriptInterop: JS - Blazor has sent us a message: ${message} - Remove Me` );
    // alert(`Interop working! ${message}`);

    console.log( `                         JS - We are sending blazor a message: hello blazor!` );
    dotNetInterface.invokeMethodAsync( "TestBlazorInterop", "hello blazor!" );
}

//
//  Action to happen when the page is unloaded. (e.g., user hits f5);
//
window.addEventListener("beforeunload",
    async function (event)
    {
        console.log("Remove Me: JS - calling Blazor's HasSpreadSheetChanged");

        var changed = await dotNetInterface.invokeMethodAsync("HasSpreadSheetChanged");

        if (changed)
        {
            // Set a custom message
            event.preventDefault();
            event.returnValue = 'Are you sure you want to leave?';
            return 'Are you sure you want to leave?';
        }
        else
        {
            console.log("Remove Me: JS - The spreadsheet was not changed, so we can navigate elsewhere.");
        }
    });

/**
 * Save the data into the file.
 * @param {string} fileName - the name of the file to save to.
 * @param {string} fileContent - the data that goes in the file.
 */
export function saveToFile(fileName, fileContent)
{
    try
    {
        console.log(`download file ${fileName} - Remove Me` );
        // Create a blob with the file content
        const blob = new Blob([fileContent], { type: "text/plain" });

        // Create a link element
        const a = document.createElement("a");
        a.href = URL.createObjectURL(blob);
        a.download = fileName;

        // Append the anchor element to the body
        document.body.appendChild(a);

        // Click the link to trigger download
        a.click();

        // Remove the anchor element from the DOM
        document.body.removeChild(a);

        return true;
    }
    catch
    {
        return false;
    }
}