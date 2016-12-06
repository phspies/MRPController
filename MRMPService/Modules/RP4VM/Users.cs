
using MRMPService.RP4VMAPI;
using System.Collections.Generic;

namespace MRMPService.RP4VMTypes
{
    public class Users : Core
    {
        public Users(RP4VM_ApiClient _RP4VM) : base(_RP4VM) { }

        public void setPassword_Method(setPasswordParams setPasswordParams_object)
        {
            endpoint = "/users/password";
            mediatype = "*/*";
            put(setPasswordParams_object);
        }


        public snmpUserSet getAllSNMPUsers_Method()
        {
            endpoint = "/users/snmp";
            mediatype = "application/json";
            return get<snmpUserSet>();
        }


        public UserRole getUserRole_Method(string roleName)
        {
            endpoint = "/users/roles/{roleName}";
            endpoint.Replace("{roleName}", roleName.ToString());
            mediatype = "application/json";
            return get<UserRole>();
        }


        public SNMPUser getSNMPUser_Method(string userName)
        {
            endpoint = "/users/snmp/{userName}";
            endpoint.Replace("{userName}", userName.ToString());
            mediatype = "application/json";
            return get<SNMPUser>();
        }


        public void removeRecoverPointUsers_Method(recoverPointUserSet recoverPointUserSet_object)
        {
            endpoint = "/users/rp_users";
            mediatype = "*/*";
            delete(recoverPointUserSet_object);
        }


        public RecoverPointUser getRecoverPointUser_Method(string userName)
        {
            endpoint = "/users/rp_users/{userName}";
            endpoint.Replace("{userName}", userName.ToString());
            mediatype = "application/json";
            return get<RecoverPointUser>();
        }


        public UserInformation getCurrentUserInformation_Method()
        {
            endpoint = "/users/current";
            mediatype = "application/json";
            return get<UserInformation>();
        }


        public void editRecoverPointUser_Method(string oldUserName, RecoverPointUser recoverPointUser_object)
        {
            endpoint = "/users/rp_users/{oldUserName}";
            endpoint.Replace("{oldUserName}", oldUserName.ToString());
            mediatype = "*/*";
            put(recoverPointUser_object);
        }


        public void addRecoverPointUsers_Method(recoverPointUserSet recoverPointUserSet_object)
        {
            endpoint = "/users/rp_users/add_users";
            mediatype = "*/*";
            post(recoverPointUserSet_object);
        }


        public void setUsersSettings_Method(UsersSettings usersSettings_object)
        {
            endpoint = "/users/settings";
            mediatype = "*/*";
            put(usersSettings_object);
        }


        public UserEventLogsFilter getCurrentUserEventLogsFilter_Method()
        {
            endpoint = "/users/current/event_logs_filter";
            mediatype = "application/json";
            return get<UserEventLogsFilter>();
        }


        public userRoleSet getAllUserRoles_Method()
        {
            endpoint = "/users/roles";
            mediatype = "application/json";
            return get<userRoleSet>();
        }
    }
}
