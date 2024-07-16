import { createRoot } from 'react-dom/client';

import { AppComponent } from './components/app.component';

createRoot(document.getElementById('react-root') as HTMLElement).render(<AppComponent />);