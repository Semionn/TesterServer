using System;
using System.Collections.Generic;
using System.IdentityModel.Selectors;
using System.Linq;
using System.ServiceModel;
using System.Web;

namespace WcfService1
{
    public class UserValidator : UserNamePasswordValidator
    {
            public override void Validate(string userName, string password)
            {
                if (null == userName || null == password)
                {
                    throw new ArgumentNullException();
                }

                if (!(userName == "admin" && password == "password" || password == "1234"))
                {
                    throw new FaultException("Unknown Username or Incorrect Password");
                }
            }
        }
    }
