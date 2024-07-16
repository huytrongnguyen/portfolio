import { ReactNode } from 'react';
import { Link } from 'react-router-dom';
import pkg from '../../../../../package.json';
import { Navigation } from './navigation.component';

export function AppHeader(props: { children?: ReactNode | undefined }) {
  return <div className="navbar border-bottom p-0">
    <div className="container-fluid justify-content-start">
      <Link to="home" className="navbar-brand">
        <span className="fw-bold">Portfolio</span>
        <small className="text-secondary"> v{pkg.version}</small>
      </Link>
      {props.children}
      <Link to={`/import`} className="btn btn-sm btn-outline-secondary me-2">
        <span className="fa fa-cloud-arrow-up me-1" /> Import Data
      </Link>
      <Navigation />
    </div>
  </div>
}