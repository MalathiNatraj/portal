using System;
using System.ComponentModel;

namespace Diebold.Domain.Entities
{
    public class HistoryLog : IntKeyedEntity
    {
        public virtual DateTime Date { get; set; }
        public virtual User User { get; set; }
        public virtual LogAction Action { get; set; }
        public virtual string Description { get; set; }

        public virtual CompanyGrouping1Level CompanyGrouping1Level { get; set; }
        public virtual CompanyGrouping2Level CompanyGrouping2Level { get; set; }
        public virtual Site Site { get; set; }
        public virtual Device Device { get; set; }
    }

    public enum LogAction
    {
        [Description("All")] All,
        //LOG001 As a user, I want to sign in to the application
        [Description("Sign In")] SignIn,
        //LOG002 As a user, I want sign out the application
        [Description("Sign Out")] SignOut,

        //USR002 As a user, I want to sign in as another user
        SignAsAnotherUser,
        //USR003 As a user, I want to edit an existing user
        [Description("Edit User")] UserEdit,
        //USR004 As a user, I want to enable/disable a user account
        [Description("Enable User")] UserEnable,
        [Description("Disable User")] UserDisable,
        //USR005 As a user, I want to delete a user account
        [Description("Delete User")] UserDelete,
        //USR007 As a user, I want to create a user
        [Description("Create User")] UserCreate,
        //USR008 As a user, I want to assign the monitors to a Customer User
        [Description("Assign Monitors to User")] UserAssignMonitor,
        //USR009 As a user, I want to opt in/out notifications
        [Description("Opt In Notification for User")] UserOptInNitification,
        [Description("Opt Out Notification for User")] UserOptOutNitification,

        //ROL002 As a user, I want to edit an existing role
        [Description("Edit Role")] RoleEdit,
        //ROL003 As a user, I want to delete a role
        [Description("Delete Role")] RoleDelete,
        //ROL004 As a user, I want to create a role
        [Description("Create Role")] RoleCreate,

        //ALE001 As a user, I want to configure the default alerts
        [Description("Configure Default Alerts")] AlertsConfigureDefault,
        
        //COM002 As a user, I want to edit an existing company
        [Description("Edit Company")] CompanyEdit,
        //COM003 As a user, I want to enable/disable a company
        [Description("Enable Company")] CompanyEnable,
        [Description("Disable Company")] CompanyDisable,
        //COM004 As a user, I want to delete a company
        [Description("Delete Company")] CompanyDelete,
        //COM006 As a user, I want to create a company
        [Description("Create Company")] CompanyCreate,
        //COM007 As a user, I want to add a new first level group.
        [Description("Add First Level Group for Company")] CompanyAddFirstLevelGroup,
        //COM008 As a user, I want to add a new second level group.
        [Description("Add Second Level Group for Company")] CompanyAddSecondLevelGroup,
        //COM009 As a user, I want to group sites into 2nd and 1st level groupings.
        [Description("Group Sites for Company")] CompanyGroupSites,

        //SIT003 As a user, I want to edit an existing site
        [Description("Edit Site")] SiteEdit,
        //SIT004 As a user, I want to enable/disable a site
        [Description("Enable Site")] SiteEnable,
        [Description("Disable Site")] SiteDisable,
        //SIT005 As a user, I want to delete a site
        [Description("Delete Site")] SiteDelete,
        //SIT007 As a user, I want to create a site
        [Description("Create Site")] SiteCreate,

        //GAT002 As a user, I want to edit an existing gateway
        [Description("Edit Gateway")] GatewayEdit,
        //GAT003 As a user, I want to enable/disable a gateway
        [Description("Enable Gateway")] GatewayEnable,
        [Description("Disable Gateway")] GatewayDisable,
        //GAT004 As a user, I want to delete a gateway
        [Description("Delete Gateway")] GatewayDelete,
        //GAT006 As a user, I want to create a gateway
        [Description("Create Gateway")] GatewayCreate,
        //GAT007 As a user, I want to test the connection of a gateway
        [Description("Test Connection for Gateway")] GatewayTestConnection,
        //GAT008 As a user, I want to reload a gateway
        [Description("Reload Gateway")] GatewayReload,
        //GAT009 As a user, I want to audit a gateway
        [Description("Audit Gateway")] GatewayAudit,
        //GAT010 As a user, I want to force a gateway update
        [Description("Force Update for Gateway")] GatewayForceUpdate,
        //GAT011 As a user, I want to restart a gateway
        [Description("Restart Gateway")] GatewayRestart,
        //GAT004 As a user, I want to revoke a gateway
        [Description("Revoke Gateway")] GatewayRevoke,

        //DEV002 As a user, I want to edit an existing device
        //DEV012 As a User I want to be able to specify cameras name, port and status
        [Description("Edit Device")] DeviceEdit,
        //DEV003 As a user, I want to enable/disable a device
        [Description("Enable Device")] DeviceEnable,
        [Description("Disable Device")] DeviceDisable,
        //DEV004 As a user, I want to delete a device
        [Description("Delete Device")] DeviceDelete,
        //DEV006 As a user, I want to create a device
        [Description("Create Device")] DeviceCreate,
        //DEV007 As a user, I want to test the connection of a device
        [Description("Test Connection Device")] DeviceTestConnection,
        //DEV008 As a user, I want to reload a device
        [Description("Reload Device")] DeviceReload,
        //DEV009 As a user, I want to audit a device
        [Description("Audit Device")] DeviceAudit,
        //DEV010 As a user, I want to force a device update
        [Description("Force Update for Device")] DeviceForceUpdate,
        //DEV011 As a user, I want to restart a device
        [Description("Restart Device")] DeviceRestart,

        //DAS005 As a user, I want to acknowledge an alert
        [Description("Acknowledge Alert")] AlertAcknowledge,
        //DAS006 As a user, I want to add a note to an alert
        [Description("Add a Note to a Device")] NoteCreate,

        //VID001 As a user, I want to view the video of a device
        [Description("View Video")] VideoView,
    }
}
