import React from "react";
import { withRouter } from "react-router";
import styled from "styled-components";

const Styled = styled.nav`
  &.Navigation {
      .secondary {
      font-family: SegoeUISemilight, 'Helvetica Neue', Helvetica, Arial,
        sans-serif;
      list-style: none;
      margin: 0;
      padding: 0 2rem;
      font-size: 1.125rem;
      height: 50px;
      display: flex;
      align-items: center;
      justify-content: flex-start;

      li {
        margin-right: 4rem;

        span {
          display: inline-block;
          padding-bottom: 1px;
        }

        &.active {
          span {
            border-bottom: 2px solid ${props => props.theme.color_primary};
          }
        }
      }
    }

    .breadcrumbs {
      font-family: SegoeUISemibold, 'Helvetica Neue', Helvetica, Arial,
        sans-serif;
      list-style: none;
      margin: 0;
      padding: 0 2rem;
      font-size: 0.875rem;
      height: 35px;
      display: flex;
      align-items: flex-start;
      justify-content: flex-start;

      li {
        padding: 0 25px;
        position: relative;

        &::after {
          position: absolute;
          top: 0;
          right: 0;
          color: ${props => props.theme.color_secondary}
          content: '>';
        }

        &:first-child {
          padding-left: 0;
        }
        &:last-child::after {
          content: '';
        }
      }
    }
  }
`;

const Navigation = ({ location }) => {
  const isBreadcrumbsVisible = location.pathname === "/permit-request";
  return (
    <Styled className="Navigation">
      <ul className="secondary">
        <li className={isBreadcrumbsVisible ? "active" : ""}>
          <span>Services</span>
        </li>
        <li>
          <span>Events</span>
        </li>
        <li>
          <span>Recreation</span>
        </li>
        <li>
          <span>Have Your Say</span>
        </li>
        <li>
          <span>Your Council</span>
        </li>
      </ul>
      {isBreadcrumbsVisible && (
        <ol className="breadcrumbs">
          <li>
            <span>Home</span>
          </li>
          <li>
            <span>Services</span>
          </li>
          <li>
            <span>Building Permits</span>
          </li>
          <li>
            <span>Permit Requirements</span>
          </li>
        </ol>
      )}
    </Styled>
  );
};

export default withRouter(Navigation);
