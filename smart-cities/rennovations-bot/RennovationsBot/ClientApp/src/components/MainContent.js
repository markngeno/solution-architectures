import React from "react";
import styled from "styled-components";

const Styled = styled.main`
  &.MainContent {
  }
`;

const MainContent = ({ children }) => (
  <Styled className="MainContent">{children}</Styled>
);

export default MainContent;
