import { useState } from 'react';
import { Link, useLocation } from 'react-router-dom';
import { Rosie } from 'rosie-ui';

export type NavItem = {
  navId: string,
  navName: string,
  children?: NavItem[],
}

export function Navigation() {
  const location = useLocation(),
        [navigation] = useState<NavItem[]>([]);

  return <>
    <button className="navbar-toggler" type="button" data-bs-toggle="dropdown">
      <span className="navbar-toggler-icon"></span>
    </button>
    <div className="dropdown-menu dropdown-menu-end">
      <div className="container">
        <div className="row">
          {navigation.map(navItem => {
            if (navItem?.children?.length) {
              return <div key={navItem.navId} className="col-auto">
                <div className="nav flex-column">
                  <span className="nav-item fw-bold">{navItem.navName}</span>
                  {navItem.children.map(childNavItem => {
                    return <Link key={childNavItem.navId} to={childNavItem.navId} className={Rosie.classNames('nav-link p-0', { active: location?.pathname.startsWith(childNavItem.navId) })}>
                      {childNavItem.navName}
                    </Link>
                  })}
                </div>
              </div>
            }

            return <div key={navItem.navId} className="col-auto">
              <div className="nav flex-column">
                <Link to={navItem.navId} className={Rosie.classNames('nav-link p-0', { active: location?.pathname.startsWith(navItem.navId) })}>{navItem.navName}</Link>
              </div>
            </div>
          })}
        </div>
      </div>
    </div>
  </>
}