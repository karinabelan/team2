using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace team2
{
    public class Team2Validate : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            // Obtain the tracing service
            ITracingService tracingService =
            (ITracingService)serviceProvider.GetService(typeof(ITracingService));

            // Obtain the execution context from the service provider.  
            IPluginExecutionContext context = (IPluginExecutionContext)
                serviceProvider.GetService(typeof(IPluginExecutionContext));

            // The InputParameters collection contains all the data passed in the message request.  
            if (context.InputParameters.Contains("Target") &&
                context.InputParameters["Target"] is Entity)
            {
                // Obtain the target entity from the input parameters.  
                Entity entity = (Entity)context.InputParameters["Target"];

                // Obtain the organization service reference which you will need for  
                // web service calls.  
                IOrganizationServiceFactory serviceFactory =
                    (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
                IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);

                try
                {
                    var userId= entity.GetAttributeValue<EntityReference>("t2_user").Id;
                    var query = $@"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                                   <entity name='t2_terminaluserteam2'>
                                     <attribute name='t2_terminaluserteam2id' />
                                     <filter type='and'>
                                       <condition attribute='t2_user' operator='eq' value='{userId}' />
                                     </filter>
                                   </entity>
                                 </fetch>";
                    var fetchXml = new FetchExpression(query);
                    var response = service.RetrieveMultiple(fetchXml);
                    if (response !=null && response.Entities.Any())
                    {
                        throw new InvalidPluginExecutionException("This user already has a terminal");
                    }
                }

                catch (FaultException<OrganizationServiceFault> ex)
                {
                    throw new InvalidPluginExecutionException("An error occurred in FollowUpPlugin.", ex);
                }

                catch (Exception ex)
                {
                    tracingService.Trace("FollowUpPlugin: {0}", ex.ToString());
                    throw;
                }
            }
        }
    }
}
