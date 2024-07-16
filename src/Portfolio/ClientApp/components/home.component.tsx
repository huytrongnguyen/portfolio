import { AppBody, AppHeader } from './ux';

export function HomeComponent() {
  return <>
    <AppHeader>
      <ol className="breadcrumb flex-fill border-start">
        <li className="breadcrumb-item active">Home</li>
        <div className="ms-auto"></div>
      </ol>
    </AppHeader>
    <AppBody></AppBody>
  </>
}