import React, { Component } from "react";
import styled from "styled-components";
import Logo from "../icons/city-seal_large";
import SettingsIcon from "../icons/settings-icon";
import Upload from "../icons/settings-icon";
import config from "../common/config";

const Styled = styled.div`
  &.login-background {
    position: absolute;
    top: 0;
    left: 0;
    height: 100vh;
    width: 100vw;
    background-color: #f5f6fa;
    z-index: 100;
  }

  .login-modal {
    display: flex;
    flex-direction: column;
    justify-content: center;
    align-items: center;
    position: absolute;
    top: 50%;
    left: 50%;
    transform: translate(-50%, -50%);
    width: 200px;
    padding: 30px 50px;
    border-radius: 2px;
    background-color: #003da5;

    .logo {
      width: 175px;
      height: 175px;
      margin-bottom: 10px;
    }
  }

  .login-logo {
    width: 80%;
    margin: 14px 0 28px 0;
  }

  select,
  input[type="text"],
  input[type="password"] {
    width: 100%;
    margin: 10px 0;
    border: 0 none;
    padding: 5px 10px;
    line-height: 1.4;
    box-sizing: border-box;
    border: 1px solid #003da5;
  }

  .settings {
    display: flex;
    align-items: center;
    color: #ffffff;
    margin: 14px 0 24px;
    width: 100%;
    font-size: 14px;

    div {
      flex-grow: 1;
      padding-right: 10px;

      input[type="checkbox"] {
        display: inline-block;
        margin-right: 10px;
      }

      span {
        vertical-align: -1px;
      }
    }

    .cog-icon {
      display: inline-block;
      width: 1rem;
      height: 1rem;
      fill: #ffffff;
      cursor: pointer;
    }
  }

  button {
    display: block;
    color: #ffffff;
    background-color: #009cff;
    height: 32px;
    width: 100%;
    font-size: 14px;
  }

  .login-forgot {
    color: #ffffff;
    font-size: 14px;
    margin-top: 20px;
  }

  .login-settings-modal-background {
    position: absolute;
    left: 270px;
    top: 160px;
    padding: 30px;
    background-color: white;
    box-shadow: 0 4px 12px 0 rgba(0, 0, 0, 0.05);

    .login-settings-modal {
      display: flex;
      flex-direction: column;
      justify-content: center;
      align-items: center;
      width: 170px;
    }

    .preview-image {
      width: 60%;
      margin: 10px 0;
    }

    .label-button {
      width: 100%;
      margin-bottom: 10px;
      color: #009cff;
      font-size: 14px;
      display: flex;
      align-items: center;
      cursor: pointer;
    }

    .upload-icon {
      margin-right: 10px;
      display: inline-block;
      width: 1rem;
      height: 1rem;
      fill: #009cff;
    }

    input[type="file"] {
      position: absolute;
      left: -3000px;
    }

    select {
      border: 0;
      outline: 1px solid #003da5;
      background-color: white;
      color: gray;
      height: 34px;
    }

    input[type="text"] {
      margin-bottom: 20px;
    }
  }
`;

class Login extends Component {
  currentUser = {
    name: config.USER_NAME,
    image: null
  };

  state = {
    openSettingsModal: false,
    previewImage: null
  };

  updateName = e => (this.currentUser.name = e.target.value);

  updatePhoneNumber = event => {
    let newValue = event.target.value;

    // Strip all characters from the input except digits
    newValue = newValue.replace(/\D/g, "");
    newValue = newValue.substr(0, 16);

    const size = newValue.length;
    let number = "";

    if (size > 0) {
      number += `+${newValue.substr(0, 1)}`;
    }
    if (size > 1) {
      number += ` ${newValue.substr(1, 3)}`;
    }
    if (size > 4) {
      number += ` ${newValue.substr(4, 3)}`;
    }
    if (size > 7) {
      number += ` ${newValue.substr(7, 4)}`;
    }
    if (size > 11) {
      number += ` ${newValue.substr(11, 5)}`;
    }

    this.currentUser.formattedPhoneNumber = number;
    this.currentUser.phoneNumber = newValue;
  };

  updateImage = e => {
    const file = e.target.files[0];
    const reader = new FileReader();
    reader.onloadend = () => {
      this.currentUser.image = reader.result;
      this.setState({ previewImage: reader.result });
    };
    reader.readAsDataURL(file);
  };

  updateLocation = e => {
    this.currentUser.location = e.target.value;
  };

  login = () => {
    this.props.onLogin(this.currentUser);
  };

  toggleSettingsModal = () => {
    this.setState(prevState => {
      return { openSettingsModal: !prevState.openSettingsModal };
    });
  };

  render() {
    const { previewImage, openSettingsModal } = this.state;
    return (
      <Styled className="login-background">
        <div className="login-modal">
          <Logo className="logo" />
          <input
            className="login-input"
            type="text"
            placeholder="User"
            onChange={this.updateName}
          />
          <input
            className="login-input"
            type="password"
            placeholder="Password"
          />
          <div className="settings">
            <div>
              <input type="checkbox" />
              <span>Keep me logged in</span>
            </div>
            <label>
              <SettingsIcon
                className="cog-icon"
                onClick={this.toggleSettingsModal}
              />
            </label>
          </div>
          {openSettingsModal && (
            <div className="login-settings-modal-background">
              <div className="login-settings-modal">
                <label className="label-button">
                  <Upload className="upload-icon" /> Upload image
                  <input
                    id="upload-image"
                    type="file"
                    onChange={this.updateImage}
                  />
                </label>
                {previewImage && (
                  <img
                    src={previewImage}
                    className="preview-image"
                    alt="preview"
                  />
                )}
                <select value={this.state.value} onChange={this.updateLocation}>
                  <option value="en-US">US Based</option>
                  <option value="en-GB">Non-US Based</option>
                </select>
                <input
                  className="login-input"
                  type="text"
                  placeholder="Phone Number"
                  onChange={this.updatePhoneNumber}
                />
                <button type="button" onClick={this.toggleSettingsModal}>
                  Ok
                </button>
              </div>
            </div>
          )}
          <button type="button" onClick={this.login}>
            Sign In
          </button>
          <div className="login-forgot">Forgot password?</div>
        </div>
      </Styled>
    );
  }
}

export default Login;
