# Ai.Relational.Api.DefaultApi

All URIs are relative to *http://127.0.0.1:8010*

Method | HTTP request | Description
------------- | ------------- | -------------
[**TransactionPost**](DefaultApi.md#transactionpost) | **POST** /transaction | Issues a transaction to be executed



## TransactionPost

> TransactionResult TransactionPost (Transaction transaction)

Issues a transaction to be executed

### Example

```csharp
using System.Collections.Generic;
using System.Diagnostics;
using Ai.Relational.Api;
using Ai.Relational.Client;
using Ai.Relational.Model;

namespace Example
{
    public class TransactionPostExample
    {
        public static void Main()
        {
            Configuration.Default.BasePath = "http://127.0.0.1:8010";
            var apiInstance = new DefaultApi(Configuration.Default);
            var transaction = new Transaction(); // Transaction | Optional description in *Markdown*

            try
            {
                // Issues a transaction to be executed
                TransactionResult result = apiInstance.TransactionPost(transaction);
                Debug.WriteLine(result);
            }
            catch (ApiException e)
            {
                Debug.Print("Exception when calling DefaultApi.TransactionPost: " + e.Message );
                Debug.Print("Status Code: "+ e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

### Parameters


Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **transaction** | [**Transaction**](Transaction.md)| Optional description in *Markdown* | 

### Return type

[**TransactionResult**](TransactionResult.md)

### Authorization

No authorization required

### HTTP request headers

- **Content-Type**: application/json
- **Accept**: application/json

### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** | A successful result is wrapped inside a TransactionResult |  -  |
| **0** | All errors are also wrapped inside a TransactionResult |  -  |

[[Back to top]](#)
[[Back to API list]](../README.md#documentation-for-api-endpoints)
[[Back to Model list]](../README.md#documentation-for-models)
[[Back to README]](../README.md)

