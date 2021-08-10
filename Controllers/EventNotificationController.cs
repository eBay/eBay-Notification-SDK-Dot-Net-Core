/*
 * Copyright (c) 2021 eBay Inc.
 *
 *  Licensed under the Apache License, Version 2.0 (the "License");
 *  you may not use this file except in compliance with the License.
 *  You may obtain a copy of the License at
 *  http://www.apache.org/licenses/LICENSE-2.0
 *
 *  Unless required by applicable law or agreed to in writing, software
 *  distributed under the License is distributed on an "AS IS" BASIS,
 *  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *  See the License for the specific language governing permissions and
 *  limitations under the License.
 *
 */

using System;
using EbayEventNotificationSDK.Models;
using EbayEventNotificationSDK.Processor;
using EbayEventNotificationSDK.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace EbayEventNotificationSDK.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class EventNotificationController : ControllerBase
    {
        private readonly ILogger<EventNotificationController> logger;

        private readonly ISignatureValidator signatureValidator;

        private readonly IEndPointValidator endpointValidator;

        public EventNotificationController(ILogger<EventNotificationController> logger, ISignatureValidator validator, IEndPointValidator endPointValidator)
        {
            this.logger = logger;
            this.signatureValidator = validator;
            this.endpointValidator = endPointValidator;
        }


        [HttpPost]
        [Route("webhook")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status412PreconditionFailed)]
        public ActionResult process([FromBody] Message message, [FromHeader(Name = "X-EBAY-SIGNATURE")] String signatureHeader)
        {
            try
            {
                if (signatureValidator.validate(message, signatureHeader))
                {
                    process(message);
                    return StatusCode(204);
                }
                else
                {
                    return StatusCode(412);
                }
            }
            catch (Exception e)
            {
                logger.LogError("Signature validation processing failure:" + e.Message);
                return StatusCode(500);
            }
        }

        [HttpGet]
        [Route("webhook")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<ChallengeResponse> validate([FromQuery(Name = "challenge_code")] string challengeCode)
        {
            try
            {
                return endpointValidator.generateChallengeResponse(challengeCode);
            }
            catch (Exception ex)
            {
                logger.LogError("Endpoint validation failure:" + ex.Message);
                return StatusCode(500);
            }
        }

        private void process(Message message)
        {
            TopicEnum topicEnum = (TopicEnum)Enum.Parse(typeof(TopicEnum), message.metadata.topic);
            MessageProcessorFactory.getProcessor(topicEnum).process(message);
        }
}

}