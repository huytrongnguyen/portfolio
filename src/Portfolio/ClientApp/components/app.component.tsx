import { BrowserRouter as Router, Navigate, Route, Routes } from 'react-router-dom';
import { ImportComponent } from './exim';
import { HomeComponent } from './home.component';

export function AppComponent() {
  return <Router>
    <div className="app d-flex flex-column">
      <Routes>
        <Route path="/home" element={<HomeComponent />} />
        <Route path="/import" element={<ImportComponent />} />
        <Route path="*" element={<Navigate to="/home" />} />
      </Routes>
    </div>
  </Router>
}