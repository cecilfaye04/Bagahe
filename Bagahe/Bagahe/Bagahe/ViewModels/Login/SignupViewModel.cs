﻿using Acr.UserDialogs;
using Bagahe.Interfaces;
using Bagahe.Models;
using MvvmCross.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace Bagahe.ViewModels.Login
{
    class SignupViewModel : BaseViewModel
    {
        private readonly IUserDialogs _udialog;
        private readonly ISignupService _service;


        public SignupViewModel(ISignupService service, IUserDialogs dialog)
        {
            _udialog = dialog;
            _service = service;
        }

        public ICommand SignUpCommand
        {
            get
            {
                
                return new MvxCommand(async () => {
                    SignupErrMsg.Init();
                    bool isSuccess;   
                    bool isValidInput = checkInput();
                    if (isValidInput)
                    {
                        isSuccess = await _service.AddNewUser(SignupFields);
                        if (isSuccess)
                        {
                            ShowViewModel<SignupResultViewModel>();
                        } else
                        {
                            SignUpGeneralErrorMsg = "The application has encountered an unknown error.";
                        }
                    }
                });
            }
        }

        int countInvalidInput;
        private bool checkInput()
        {
            countInvalidInput = 0;
            bool result = true;
            SignUpGeneralErrorMsg = "";
            //Check inputs if empty.
            //You can add additional conditions for the inputs here.
            if (isEmpty(SignupFields.FirstName))
                SignupErrMsg.FirstNameErrMsg = "First name is required.";
            if (isEmpty(SignupFields.LastName))
                SignupErrMsg.LastNameErrMsg = "Last name is required.";
            if (isEmpty(SignupFields.Username))
                SignupErrMsg.UsernameErrMsg = "Username is required.";

            //Validate Password and email
            isValidPassword(SignupFields.Password);
            isValidEmail(SignupFields.Email);

            if (isEmpty(SignupFields.RePassword))
            {
                SignupErrMsg.RePasswordErrMsg = "Please re-enter your password.";
            }
            else
            {
                if (!SignupFields.RePassword.Equals(SignupFields.Password))
                {
                    SignupErrMsg.RePasswordErrMsg = "The password you have entered doesn't match.";
                    countInvalidInput++;
                }
            }
            
            if (countInvalidInput != 0)
            {
                SignUpGeneralErrorMsg = "Please correct the " + countInvalidInput + " item";
                if (countInvalidInput != 1)
                {
                    SignUpGeneralErrorMsg += "s";
                }
                result = false;
            }
            
            return result;
        }

        private bool isEmpty(string input)
        {
            bool result = false;
            if (("").Equals(input) || input == null)
            {
                countInvalidInput++;
                result = true;
            }
            return result;
        }
        private void isValidPassword(string password)
        {
            if (isEmpty(password))
            {
                SignupErrMsg.PasswordErrMsg = "Password is required.";
            } else if (password.Length < 8)
            {

                SignupErrMsg.PasswordErrMsg = "Passwords must be at least 8 characters long.";
            }
            else 
            {

                if (!Regex.IsMatch(password, @"[A-Z]"))
                {
                    SignupErrMsg.PasswordErrMsg = "Your password must contain an uppercase letter.";
                    countInvalidInput++;
                }
                else if (!Regex.IsMatch(password, @"[a-z]"))
                {
                    SignupErrMsg.PasswordErrMsg = "Your password must contain a lowercase letter.";
                    countInvalidInput++;
                }
                else if (!Regex.IsMatch(password, @"[0-9]"))
                {
                    SignupErrMsg.PasswordErrMsg = "Your password must contain a number.";
                    countInvalidInput++;
                }
            }
        }


        private void isValidEmail(string email)
        {
            if (isEmpty(email))
            {
                SignupErrMsg.EmailErrMsg = "The email address is required.";
            }
            else
            {
                string regex = @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z";
                bool isEmail = Regex.IsMatch(email, regex, RegexOptions.IgnoreCase);
                if (!isEmail)
                {
                    SignupErrMsg.EmailErrMsg = "The email address you entered is not valid.";
                    countInvalidInput++;
                }
            }

            
        }

        private SignupErrMsg _signupErrMsg = new SignupErrMsg();
        public SignupErrMsg SignupErrMsg
        {
            get { return _signupErrMsg; }
            set
            {
                _signupErrMsg = value;
                RaisePropertyChanged(() => SignupErrMsg);
            }
        }

        private SignupFields _signupFields = new SignupFields();
        public SignupFields SignupFields
        {
            get { return _signupFields; }
            set
            {
                _signupFields = value;
                RaisePropertyChanged(() => SignupFields);
            }
        }
        private string _signUpGeneralErrorMsg;
        public string SignUpGeneralErrorMsg
        {
            get { return _signUpGeneralErrorMsg; }
            set
            {
                _signUpGeneralErrorMsg = value;
                RaisePropertyChanged(() => SignUpGeneralErrorMsg);
            }
        }

        
    }
}
