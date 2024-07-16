import { ReactNode } from 'react';

export function AppBody(props: { children?: ReactNode | undefined }) {
  return <div className="app-wrapper d-flex position-relative">
    <div id="app-splash-screen" className="mask">
      <div className="mask-msg">
        <div className="mask-msg-text">
          <span className="fa fa-circle-notch fa-spin me-1" />
          Loading...
        </div>
      </div>
    </div>
    <div className="app-body d-flex flex-column">
      {props.children}
    </div>
  </div>
}