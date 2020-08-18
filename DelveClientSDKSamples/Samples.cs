using System;

namespace DelveClientSDKSamples
{
    class Sample
    {
        static void Main(string[] args)
        {
            LocalWorkflow workflow = new LocalWorkflow();
            workflow.runLocalWorkflow();

            CloudWorkflow cloudWorkflow = new CloudWorkflow();
            cloudWorkflow.runCloudWorkflow();
        }
    }
}