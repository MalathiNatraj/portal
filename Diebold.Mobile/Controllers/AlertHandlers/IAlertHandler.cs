using System.Collections.Generic;
using Diebold.Domain.Entities;
using DieboldMobile.Models;

namespace DieboldMobile.Controllers.AlertHandlers
{
    //Strategy to define different behaviors to handle alerts.
    public interface IAlertHandler
    {
        /// <summary>
        /// Defines if a value should be ignored to generate an alert or not.
        /// </summary>
        /// <param name="value">Value to check.</param>
        /// <returns>True if value is ignored.</returns>
        bool IsIgnoredValue(string value);

        /// <summary>
        /// Defines if a value satisfies the rules defined by the handler,
        /// according to a threshold and a relational operator.
        /// </summary>
        /// <param name="value">Value to check.</param>
        /// <param name="threshold">Second value to be compared by the operation.</param>
        /// <param name="relationalOperator">Relational operator which will compare values.</param>
        /// <returns></returns>
        bool SatisfiesRule(string value, object threshold, AlarmOperator relationalOperator);

        /// <summary>
        /// Creates the structures needed to generate the local alerts according to the one which was received.
        /// </summary>
        /// <param name="alert">Alert received from platform.</param>
        /// <returns>List of alert structures to create local alerts.</returns>
        List<AlertInfo> HandleAlert(Alert alert);

        /// <summary>
        /// Defines if the value of a drive indicates whether it is connected or not.
        /// </summary>
        /// <param name="value">Value to check.</param>
        /// <returns>True if it is connected.</returns>
        bool SatisfiesCapabilityRule(string value);

        /// <summary>
        /// Create alerts locally or updates their status.
        /// 
        /// Update device status.
        /// 
        /// (can't be separated because it should be done in same service method because of transactions)
        /// </summary>
        /// <param name="alertList">List of alert structures.</param>
        void CreateAlerts(IList<AlertInfo> alertList);

        /// <summary>
        /// Sends notifications for alerts passed by parameter.
        /// </summary>
        /// <param name="alertList">List of alert structures.</param>
        void Notify(List<AlertInfo> alertList);
    }
}