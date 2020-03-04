import React from "react";
import styled from "styled-components";
import CityLogo from "../icons/city-seal_small";
import SearchIcon from "../icons/search_icon";
import CaretIcon from "../icons/caret_icon";
import ProfileIcon from "../icons/profile_icon";

const Styled = styled.header`
  &.Header {
    display: flex;
    background: ${props => props.theme.blue_gradient};
    padding: 0 2rem;
    height: ${props => props.theme.height_navbar}px;
    z-index: 10;

    .left,
    .right {
      flex: 1;
      color: ${props => props.theme.navbar_text_color};
    }

    .left {
      display: flex;
      align-items: center;
      font-size: 1.5rem;

      .CityLogo {
        width: 2.3rem;
        height: 2.3rem;
        margin-right: 15px;
      }
    }

    .right {
      display: flex;
      align-items: center;
      justify-content: flex-end;
      font-size: 1.125rem;

      .search {
        border: 1px solid white;
        border-radius: 4px;
        margin-right: 50px;
        padding-right: 8px;
        padding-left: 4px;

        input {
          background-color: transparent;
          border: 0 none;
          padding: 5px;
          width: 9rem;
          color: white;
        }

        input::placeholder {
          color: ${props => props.theme.navbar_text_color};
        }

        .SearchIcon {
          width: 1rem;
          height: 1rem;
        }
      }

      .profile {
        display: flex;
        justify-content: center;
        align-items: center;
        position: relative;

        img {
          max-width: 2rem;
          max-height: 2rem;
        }

        .ProfileIcon {
          width: 2rem;
          height: 2rem;
        }
        span {
          font-size: 1rem;
          display: inline-block;
          margin-left: 9px;
          margin-right: 12px;
        }
        .CaretIcon {
          height: 0.875rem;
          transform: rotate(90deg);
        }
        .logout-btn {
          display: flex;
          justify-content: center;
          align-items: center;
          position: absolute;
          height: 40px;
          right: -32px;
          top: 41px;
          width: 220px;
          background: ${props => props.theme.blue_gradient};
          color: ${props => props.theme.navbar_text_color};
        }
      }
    }
  }
`;

const Header = ({
  user,
  displayLogoutButton,
  onLogout,
  toggleLogoutButton
}) => (
  <Styled className="Header">
    <div className="left">
      <CityLogo className="CityLogo" />
      <span>City of Contoso</span>
    </div>
    <div className="right">
      <label className="search">
        <input placeholder="Search" name="search" />
        <SearchIcon className="SearchIcon" />
      </label>
      <div className="profile">
        {user.image ? (
          <img src={user.image} alt={user.name} />
        ) : (
          <ProfileIcon className="ProfileIcon" />
        )}
        <span>{user.name}</span>
        <button onClick={() => toggleLogoutButton()}>
          <CaretIcon className="CaretIcon" color="#fff" />
        </button>
        {displayLogoutButton && (
          <button className="logout-btn" onClick={() => onLogout()}>
            Logout
          </button>
        )}
      </div>
    </div>
  </Styled>
);

export default Header;
