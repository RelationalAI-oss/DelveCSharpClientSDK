using System;
using Com.RelationalAI;
using IniParser.Model;

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