import { useEffect, useState } from 'react';
import { Link, useLocation, useParams } from 'react-router-dom';
import { AppBody, AppHeader } from '../ux';
import { afterProcessing, alertError, beforeProcessing, query } from 'shared';
import { Ajax } from 'rosie-ui';
// import { ImportResult } from './import-result.component';
// import { RouteSelection } from './route-selection.component';

export function ImportComponent() {
  const [totalFiles, setTotalFiles] = useState(0),
        [sheetName, setSheetName] = useState('Sheet 1');

  function validate() {
    const messages: string[] = [];

    if (totalFiles < 1) messages.push('Input Files is required.');
    if (sheetName.trim() === '') messages.push('Sheet name is required.');

    if (messages.length) alertError(messages.join(' '));

    return messages;
  }

  async function onSubmit() {
    // setResults([]);

    if (validate().length) return;

    beforeProcessing();
    try {
      const formData = new FormData(query('#upload-form')[0] as HTMLFormElement),
            results = await Ajax.request<{ fileName: string, status: string, message: string }[]>({
              method: 'post',
              url: `/api/import`,
              params: {
                headers: { 'Content-Type': 'multipart/form-data' },
                body: formData,
              },
            });

      afterProcessing()
      // setResults(results);
    } catch (reason) {
      // onAjaxError(reason);
      alertError(reason);
    }
  }

  return <>
    <AppHeader>
      <ol className="breadcrumb flex-fill border-start">
        <li className="breadcrumb-item active">Import</li>
      </ol>
    </AppHeader>
    <AppBody>
      <main className="d-flex flex-row align-items-center">
        <div className="container">
          <div className="row justify-content-center">
            <div className="col-6">
              <form id="upload-form">
                <input type="hidden" name="scenario" value="Daily Reporting" />
                <div className="row">
                  <label htmlFor="file" className="col-3 col-form-label text-end fw-bold">
                    Input Files <span className="text-danger">(*)</span>
                  </label>
                  <div className="col-9">
                    <input className="form-control" type="file" name="file" id="upload-file"
                        accept="application/vnd.openxmlformats-officedocument.spreadsheetml.sheet, application/vnd.ms-excel"
                        onChange={e => setTotalFiles(e.target.files?.length ?? 0)} />
                  </div>
                </div>
                <div className="row mt-3">
                  <label htmlFor="sheet_name" className="col-3 col-form-label text-end fw-bold">
                    Sheet Name <span className="text-danger">(*)</span>
                  </label>
                  <div className="col-9">
                    <input className="form-control" type="text" id="sheet_name" name="sheet_name"
                        value={sheetName} onChange={e => setSheetName(e.target.value)} />
                  </div>
                </div>
              </form>
              <div className="d-flex justify-content-center mt-3">
                <button type="button" className="btn btn-sm btn-primary" disabled={totalFiles < 1} onClick={() => onSubmit()}>Import</button>
              </div>
            </div>
          </div>
        </div>
      </main>
    </AppBody>
  </>
}